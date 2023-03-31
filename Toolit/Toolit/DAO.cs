using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SystemTask = System.Threading.Tasks.Task;
using SQLite;
//using Microsoft.AppCenter;
using Xamarin.Essentials;
using Xamarin.Forms;
using Toolit.Converters;
using GeoJSON.Net.Geometry;
using Microsoft.AppCenter.Crashes;
using Toolit.Helpers;

namespace Toolit
{
    public class DAO
    {
        #region Fields

#if DEBUG
        private readonly Uri apiUri = new Uri("https://toolit-api-play.azurewebsites.net/");
#else
        private readonly Uri apiUri = new Uri("https://toolit-api-live.azurewebsites.net/");
#endif
        private const int apiVersion = 1;
        private readonly string versionHeader = "application/vnd.toolit.v" + apiVersion + "+json";
        public static DAO Instance { get; } = new DAO();
        private readonly string dbPath;
        private readonly SQLiteConnection db;
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(), //new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                new JsonI18nStringConverter(),
                //new DecimalConverter()
            }
        };

        // Polling thread fields.
        private volatile UserCache userCache; // Updated when signing in.
        private volatile TaskCompletionSource<bool> userPollComplete; // At least one user poll has been completed. Renewed on signin.
        private CancellationTokenSource userPollCancellationTokenSource;
        private ManualResetEvent userPollResetEvent;
        private volatile bool userPollAgain;

        // Subscriber lists.
        private readonly List<Subscriber<Bid>> bidsSubscribers;
        private readonly List<Subscriber<Chat>> chatsSubscribers;
        private readonly List<Subscriber<Message>> messagesSubscribers;
        private readonly List<Subscriber<Office>> officesSubscribers;
        private readonly List<Subscriber<Payment>> paymentsSubscribers;
        private readonly List<Subscriber<Rating>> ratingsSubscribers;
        private readonly List<Subscriber<Task>> tasksSubscribers;
        private readonly List<Subscriber<Craftsman>> craftsmenSubscribers;
        private readonly List<Subscriber<Ad>> adsSubscribers;

        // Upgrade required event.
        public delegate void UpgradeRequiredHandler();
        public event UpgradeRequiredHandler UpgradeRequiredEvent;
        #endregion

        #region Initialization

        private DAO()
        {
            userPollComplete = new TaskCompletionSource<bool>();
            bidsSubscribers = new List<Subscriber<Bid>>();
            chatsSubscribers = new List<Subscriber<Chat>>();
            officesSubscribers = new List<Subscriber<Office>>();
            messagesSubscribers = new List<Subscriber<Message>>();
            paymentsSubscribers = new List<Subscriber<Payment>>();
            ratingsSubscribers = new List<Subscriber<Rating>>();
            tasksSubscribers = new List<Subscriber<Task>>();
            craftsmenSubscribers = new List<Subscriber<Craftsman>>();
            adsSubscribers = new List<Subscriber<Ad>>();

            // Set db path.
            var filename = "toolit.db3";
            dbPath = Path.Combine(DependencyService.Get<ILibraryPath>().GetLibraryPath(), filename);

            // Set up db.
            db = new SQLiteConnection(dbPath);
            try
            {
                lock (db)
                {
                    db.Execute("CREATE TABLE IF NOT EXISTS user (id TEXT PRIMARY KEY, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS user_cursor (id TEXT PRIMARY KEY, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS bid (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS chat (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS craftsman (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS message (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS office (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS payment (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS rating (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS task (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                    db.Execute("CREATE TABLE IF NOT EXISTS ad (id TEXT PRIMARY KEY, collection TEXT, at DATETIME, value TEXT);");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error creating database tables: " + e.ToString());
            }
        }

        #endregion

        #region Polling
        private class UserPollResponse
        {
            public List<Bid> Bids { get; set; }
            public List<Chat> Chats { get; set; }
            public List<Office> Offices { get; set; }
            public List<Payment> Payments { get; set; }
            public List<Rating> Ratings { get; set; }
            public List<Task> Tasks { get; set; }
            public List<Craftsman> Craftsmen { get; set; }
            public List<Ad> Ads { get; set; }
            public List<Message> Messages { get; set; }
            public User User { get; set; }
        }

        private void StartPolling()
        {
            userPollCancellationTokenSource = new CancellationTokenSource();
            userPollResetEvent = new ManualResetEvent(false);

            // Start user poll.
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                UserCache cache; // Keeps reference to active user in poll (and its cursor), in case new user is signed in mid poll.

                // Start poll loop.
                while (true)
                {
                    userPollAgain = false;

                    cache = userCache;
                    if (cache != null)
                    {
                        try
                        {
                            // Do poll only if user is signed in.

                            // Call server. Cursor is never null if poll is ok.
                            var (g, c) = Send("GET", "users/" + cache.userId + "/poll", null, cache, cache.cursor);

                            var w = JsonSerializer.Deserialize<DataWrapper<UserPollResponse, Empty>>(g, jsonSerializerOptions);
                            if (w.Data != null)
                            {
                                var poll = w.Data;

                                if (poll.User != null)
                                {
                                    if (cache.User != null) // Must be same user.
                                    {
                                        cache.User.Update(poll.User); // Ignored if older.
                                    }
                                    else
                                    {
                                        cache.User = poll.User; // Insert into cache.
                                    }
                                    PutEntity(poll.User, cache); // Store in db.
                                    System.Diagnostics.Debug.WriteLine("User was provided in the poll");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("No user provided by the poll");
                                }

                                if (poll.Bids != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Bids, cache.Bids, cache, bidsSubscribers);
                                }

                                if (poll.Chats != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Chats, cache.Chats, cache, chatsSubscribers);
                                }

                                if (poll.Offices != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Offices, cache.Offices, cache, officesSubscribers);
                                }

                                if (poll.Payments != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Payments, cache.Payments, cache, paymentsSubscribers);
                                }

                                if (poll.Ratings != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Ratings, cache.Ratings, cache, ratingsSubscribers);
                                }

                                if (poll.Tasks != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Tasks, cache.Tasks, cache, tasksSubscribers);
                                }

                                if (poll.Craftsmen != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Craftsmen, cache.Craftsmen, cache, craftsmenSubscribers);
                                }

                                if (poll.Ads != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Ads, cache.Ads, cache, adsSubscribers);
                                }

                                if (poll.Messages != null)
                                {
                                    // Update cache.
                                    AddToCache(poll.Messages, cache.Messages, cache, messagesSubscribers);
                                }

                                // TODO: More fields.

                                if (c != cache.cursor)
                                {
                                    // Persist updated cursor. (Should be done after persisting all new objects to disk.) If a new user has been signed in this reference will still have the old id, which is correct.
                                    cache.cursor = c;
                                    PutEntity("user_cursor", cache.userId, DateTime.Now, cache.cursor);
                                }
                            }

                            cache.polled.TrySetResult(true);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("EXCEPTION in user poll: " + e.ToString());
                            Crashes.TrackError(e);
                        }
                    }

                    // Check that poll was not triggered while running.
                    if (userPollAgain)
                    {
                        continue;
                    }

                    userPollResetEvent.WaitOne(60000); // Wait for wake up event, or (on average) one minutes.
                    if (userPollCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    userPollResetEvent.Reset(); // Reset for next wait.
                }
            }, userPollCancellationTokenSource.Token);
        }

        public void Poll()
        {
            userPollAgain = true;
            userPollResetEvent?.Set(); // Send signal to wake thread.
        }

        #endregion

        #region Cache

        private interface IPoll
        {
            TaskCompletionSource<bool> Polled();
        }

        private class UserCache : IPoll
        {
            public string accessToken;
            public string refreshToken;
            public string userId;
            public string cursor;
            public volatile TaskCompletionSource<bool> polled;
            public User User;
            public Cache<Bid> Bids;
            public Cache<Chat> Chats;
            public Cache<Message> Messages;
            public Cache<Office> Offices;
            public Cache<Payment> Payments;
            // FXME(Jonathan): Can we remove this? It's already in the craftsman class?
            public Cache<Rating> Ratings;
            public Cache<Task> Tasks;
            public Cache<Craftsman> Craftsmen;
            public Cache<Ad> Ads;
            public Cache<User> Users;
            public string activeOffice;

            public UserCache(TaskCompletionSource<bool> polled)
            {
                this.polled = polled;
                this.Bids = new Cache<Bid>();
                this.Chats = new Cache<Chat>();
                this.Messages = new Cache<Message>();
                this.Offices = new Cache<Office>();
                this.Payments = new Cache<Payment>();
                this.Ratings = new Cache<Rating>();
                this.Tasks = new Cache<Task>();
                this.Craftsmen = new Cache<Craftsman>();
                this.Users = new Cache<User>();
                this.Craftsmen = new Cache<Craftsman>();
                this.Ads = new Cache<Ad>();
            }

            public TaskCompletionSource<bool> Polled() { return polled; }
        }

        // Helper method to update the cache fields.
        private void AddToCache<T>(List<T> incoming, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, bool sort = false, Cache<T> alternativeSource = null, bool persist = true) where T : IUpdate<T>
        {
            T[] r;
            lock (cache)
            {
                var c = cache.List;
                for (var i = 0; i < incoming.Count; i++)
                {
                    var f = false;
                    for (var j = 0; j < c.Count; j++)
                    {
                        if (incoming[i].Id == c[j].Id)
                        {
                            if (c[j].Update(incoming[i])) // Ignored if older.
                            {
                                lock (cache)
                                {
                                    cache.Updated = DateTime.UtcNow;
                                }
                                if (persist)
                                {
                                    PutEntity(c[j], ucache); // Update in db.
                                }
                            }
                            f = true;
                            break;
                        }
                    }
                    if (!f)
                    {
                        // Look in alternative source, if any.
                        if (alternativeSource != null)
                        {
                            var a = alternativeSource.List;
                            for (var j = 0; j < a.Count; j++)
                            {
                                if (a[j].Id == incoming[i].Id)
                                {
                                    a[j].Update(incoming[i]); // Update in case it is newer.
                                    incoming[i] = a[j]; // Use the same object.
                                    break;
                                }
                            }
                        }
                        if (sort)
                        {
                            var index = c.BinarySearch(incoming[i]);
                            if (index < 0)
                            {
                                index = ~index;
                            }
                            c.Insert(index, incoming[i]);
                        }
                        else
                        {
                            c.Add(incoming[i]);
                        }
                        cache.Updated = DateTime.UtcNow;
                        if (persist)
                        {
                            PutEntity(incoming[i], ucache); // Store in db.
                        }
                    }
                }
                r = c.ToArray();
            }
            if (subscribers != null)
            {
                Subscriber<T>[] h;
                lock (subscribers)
                {
                    h = subscribers.ToArray();
                }

                foreach (var handler in h)
                {
                    handler.Invoke(r, null, cache.Updated);
                }
            }
        }

        private void AddToCache<T>(ref T incoming, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, bool sort = false, Cache<T> alternativeSource = null, bool persist = true) where T : IUpdate<T>
        {
            T[] r;
            lock (cache)
            {
                var c = cache.List;

                var f = false;
                for (var j = 0; j < c.Count; j++)
                {
                    if (incoming.Id == c[j].Id)
                    {
                        if (c[j].Update(incoming)) // Ignored if older.
                        {
                            cache.Updated = DateTime.UtcNow;
                            if (persist)
                            {
                                PutEntity(c[j], ucache); // Update in db.
                            }
                        }
                        incoming = c[j]; // Update ref variable.
                        f = true;
                        break;
                    }
                }
                if (!f)
                {
                    // Look in alternative source, if any.
                    if (alternativeSource != null)
                    {
                        var a = alternativeSource.List;
                        for (var j = 0; j < a.Count; j++)
                        {
                            if (a[j].Id == incoming.Id)
                            {
                                a[j].Update(incoming); // Update in case it is newer.
                                incoming = a[j]; // Use the same object.
                                break;
                            }
                        }
                    }
                    if (sort)
                    {
                        var index = c.BinarySearch(incoming);
                        if (index < 0)
                        {
                            index = ~index;
                        }
                        c.Insert(index, incoming);
                    }
                    else
                    {
                        c.Add(incoming);
                    }
                    cache.Updated = DateTime.UtcNow;
                    if (persist)
                    {
                        PutEntity(incoming, ucache); // Store in db.
                    }
                }

                r = c.ToArray();
            }
            if (subscribers != null)
            {
                Subscriber<T>[] h;
                lock (subscribers)
                {
                    h = subscribers.ToArray();
                }

                foreach (var handler in h)
                {
                    handler.Invoke(r, null, cache.Updated);
                }
            }
        }

        // Gets entities from in-memory caches.
        private async Task<T[]> GetEntities<T>(IPoll cache, Cache<T> entities)
        {
            if (cache == null)
            {
                throw new IllegalStateException("Cache cannot be null.");
            }
            else if (entities == null)
            {
                throw new IllegalArgumentException("Entities list cannot be null.");
            }

            // Wait for first poll to complete.
            await cache.Polled().Task;

            return await SystemTask.Run(() =>
            {
                lock (entities)
                {
                    return entities.List.ToArray();
                }
            });
        }

        private T GetEntityFromCache<T>(string id, Cache<T> entities) where T : class, IUpdate<T>
        {
            if (entities == null)
            {
                return null;
            }

            T entity = null;
            lock (entities)
            {
                foreach (var x in entities.List)
                {
                    if (x.Id == id)
                    {
                        entity = x;
                        break;
                    }
                }
            }

            return entity;
        }

        private void LoadUserCache()
        {
            var at = Settings.AccessToken;
            var rt = Settings.RefreshToken;
            var userId = Settings.UserId;
            var ao = Settings.ActiveOffice;

            if (rt != null && userId != null)
            {
                var cache = new UserCache(userPollComplete) // NOTE: Uses poll complete created during startup or signout.
                {
                    accessToken = at,
                    refreshToken = rt,
                    userId = userId,
                    activeOffice = ao
                };

                if (cache.activeOffice == null)
                {
                    var (data, _) = Send("GET", "offices", null, cache);
                    var (offices, _) = Unwrap<Office[], Empty>(data);
                    if (offices.Length == 0) {
                        throw new Exception("No offices found");
                    }
                    cache.activeOffice = offices[0].Id;
                    Settings.ActiveOffice = cache.activeOffice;
                }

                bool foundCursor;
                try
                {
                    // Get user poll cursor from db.
                    var g = GetEntity<string>("user_cursor", userId); // NOTE: Comment out this on startup if you want to reset the user poll.
                    if (g != null)
                    {
                        cache.cursor = g;
                        foundCursor = true;
                    }
                    else
                    {
                        cache.cursor = ""; // Set inital cursor as blank.
                        foundCursor = false;
                    }

                    // Get user from db.
                    var u = GetEntity<User>("user", userId);
                    if (u != null)
                    {
                        cache.User = u;
                        System.Diagnostics.Debug.WriteLine("Found a user when loading the cache");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Could not find a user when loading the cache");
                    }

                    // Get bids from db.
                    var bids = new List<Bid>();
                    var bl = GetEntities<Bid>("bid", userId);
                    foreach (var x in bl)
                    {
                        bids.Add(x);
                    }
                    //bids.Sort();
                    cache.Bids = new Cache<Bid>(bids);

                    // Get chats from db.
                    var chats = new List<Chat>();
                    var chat_l = GetEntities<Chat>("chat", userId);
                    foreach (var x in chat_l)
                    {
                        chats.Add(x);
                    }
                    //chats.Sort();
                    cache.Chats = new Cache<Chat>(chats);

                    // Get offices from db.
                    var offices = new List<Office>();
                    var office_l = GetEntities<Office>("office", userId);
                    foreach (var x in office_l)
                    {
                        offices.Add(x);
                    }
                    //offices.Sort();
                    cache.Offices = new Cache<Office>(offices);

                    // Get payments from db.
                    var payments = new List<Payment>();
                    var payment_l = GetEntities<Payment>("payment", userId);
                    foreach (var x in payment_l)
                    {
                        payments.Add(x);
                    }
                    //payments.Sort();
                    cache.Payments = new Cache<Payment>(payments);

                    // Get ratings from db.
                    var ratings = new List<Rating>();
                    var rating_l = GetEntities<Rating>("rating", userId);
                    foreach (var x in rating_l)
                    {
                        ratings.Add(x);
                    }
                    //ratings.Sort();
                    cache.Ratings = new Cache<Rating>(ratings);

                    // Get tasks from db.
                    var tasks = new List<Task>();
                    var task_l = GetEntities<Task>("task", userId);
                    foreach (var x in task_l)
                    {
                        tasks.Add(x);
                    }
                    //tasks.Sort();
                    cache.Tasks = new Cache<Task>(tasks);

                    // Get craftsmen from db.
                    var craftsmen = new List<Craftsman>();
                    var craftsman_l = GetEntities<Craftsman>("craftsman", userId);
                    foreach (var x in craftsman_l)
                    {
                        craftsmen.Add(x);
                    }
                    //craftsmen.Sort();
                    cache.Craftsmen = new Cache<Craftsman>(craftsmen);

                    // Get messages from db.
                    var messages = new List<Message>();
                    var messages_l = GetEntities<Message>("message", userId);
                    foreach (var x in messages_l)
                    {
                        messages.Add(x);
                    }
                    //messages.Sort();
                    cache.Messages = new Cache<Message>(messages);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("EXCEPTION in loading user cache from db: " + e.ToString());

                    // Clear db.
                    DeleteEntities("user_cursor");
                    DeleteEntities("user");
                    DeleteEntities("bid");
                    DeleteEntities("chat");
                    DeleteEntities("office");
                    DeleteEntities("payment");
                    DeleteEntities("rating");
                    DeleteEntities("task");
                    DeleteEntities("craftsman");
                    DeleteEntities("ad");
                    DeleteEntities("message");

                    // TODO

                    // Start with new cursor.
                    cache.cursor = ""; // Set inital cursor as blank.
                    foundCursor = false;
                }

                userCache = cache;

                if (foundCursor)
                {
                    // Found cursor on disk, so initial user poll is already done.
                    if (!cache.polled.TrySetResult(true))
                    {
                        // TODO: log, this method has been called twice.
                    }
                }
            }
        }

        #endregion

        #region Persistence

        private List<T> GetEntities<T>(string table, string collection = null, string id = null) where T : class
        {
            lock (db)
            {
                List<DBQueryHelper> entities;
                if (id != null && collection != null)
                {
                    entities = db.Query<DBQueryHelper>("SELECT value AS TEXT FROM " + table + " WHERE collection = ? AND id = ?;", collection, id);
                }
                else if (id != null)
                {
                    entities = db.Query<DBQueryHelper>("SELECT value AS TEXT FROM " + table + " WHERE id = ?;", id);
                }
                else if (collection != null)
                {
                    entities = db.Query<DBQueryHelper>("SELECT value AS TEXT FROM " + table + " WHERE collection = ?;", collection);
                }
                else
                {
                    entities = db.Query<DBQueryHelper>("SELECT value AS TEXT FROM " + table + ";");
                }
                List<T> results = new List<T>(entities.Count);
                foreach (var e in entities)
                {
                    results.Add(JsonSerializer.Deserialize<T>(e.TEXT, jsonSerializerOptions));
                }
                return results;
            }
        }

        private T GetEntity<T>(string table, string id, string collection = null) where T : class
        {
            var r = GetEntities<T>(table, collection, id);
            if (r == null || r.Count == 0)
            {
                return null;
            }
            else if (r.Count == 1)
            {
                return r[0];
            }
            else //if (r.Count > 1)
            {
                throw new IllegalArgumentException("The db returned more than one result despite using id (which should be a primary key).");
            }
        }

        private int PutEntity<T>(string table, string id, DateTime at, T value, string collection = null) where T : class
        {
            var g = JsonSerializer.Serialize(value, jsonSerializerOptions);

            lock (db)
            {
                // Does this item already exist?
                DBQueryHelper h;
                if (collection != null)
                {
                    h = db.FindWithQuery<DBQueryHelper>("SELECT at AS TEXT FROM " + table + " WHERE collection = ? AND id = ?;", collection, id);
                }
                else
                {
                    h = db.FindWithQuery<DBQueryHelper>("SELECT at AS TEXT FROM " + table + " WHERE id = ?;", id);
                }
                if (h != null)
                {
                    // Check timestamp.
                    var t = Utilities.DateTimeFromRFC3339(h.TEXT);
                    if (at > t)
                    {
                        if (collection != null)
                        {
                            return db.Execute("REPLACE INTO " + table + " (id, collection, at, value) VALUES (?, ?, ?, ?);", id, collection, at.ToRFC3339(), g);
                        }
                        else
                        {
                            return db.Execute("REPLACE INTO " + table + " (id, at, value) VALUES (?, ?, ?);", id, at.ToRFC3339(), g);
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    if (collection != null)
                    {
                        return db.Execute("INSERT INTO " + table + " (id, collection, at, value) VALUES (?, ?, ?, ?);", id, collection, at.ToRFC3339(), g);
                    }
                    else
                    {
                        return db.Execute("INSERT INTO " + table + " (id, at, value) VALUES (?, ?, ?);", id, at.ToRFC3339(), g);
                    }
                }
            }
        }

        private void PutEntity<T>(T entity, UserCache cache)
        {
            if (entity is User u)
            {
                PutEntity("user", u.Id, u.Modified, u);
            }
            else if (entity is Bid b)
            {
                PutEntity("bid", b.Id, b.Modified, b, cache.userId);
            }
            else if (entity is Chat c)
            {
                PutEntity("chat", c.Id, c.Modified, c, cache.userId);
            }
            else if (entity is Office o)
            {
                PutEntity("office", o.Id, o.Modified, o, cache.userId);
            }
            else if (entity is Payment p)
            {
                PutEntity("payment", p.Id, p.Modified, p, cache.userId);
            }
            else if (entity is Rating r)
            {
                PutEntity("rating", r.Id, r.Modified, r, cache.userId);
            }
            else if (entity is Task t)
            {
                PutEntity("task", t.Id, t.Modified, t, cache.userId);
            }
            else if (entity is Craftsman cr)
            {
                PutEntity("craftsman", cr.Id, cr.Modified, cr, cache.userId);
            }
            else if (entity is Ad ad)
            {
                PutEntity("ad", ad.Id, ad.Modified, ad, cache.userId);
            }
            else if (entity is Message ms)
            {
                PutEntity("message", ms.Id, ms.Modified, ms, cache.userId);
            }
            else
            {
                throw new UnsupportedOperationException("Unsupported type (" + entity.GetType() + ").");
            }
        }

        private int DeleteEntity(string table, long id, long? collection = null)
        {
            if (collection != null)
            {
                return db.Execute("DELETE FROM " + table + " WHERE collection = ? AND id = ?;", collection.Value, id);
            }
            else
            {
                return db.Execute("DELETE FROM " + table + " WHERE id = ?;", id);
            }
        }

        private int DeleteEntities(string table, long? collection = null)
        {
            if (collection != null)
            {
                return db.Execute("DELETE FROM " + table + " WHERE collection = ?;", collection);
            }
            else
            {
                return db.Execute("DELETE FROM " + table + ";");
            }
        }

        #endregion

        #region Internal helper classes

        private class DataWrapper<T, K>
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public T Data { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public K Extra { get; set; }
        }

        private class Subscriber<T>
        {
            private readonly WeakReference reference;
            private readonly Delegate method;

            public Subscriber(Action<T[], string, DateTime> subscriber)
            {
                var target = subscriber.Target;

                if (target != null)
                {
                    // An instance method. Capture the target in a WeakReference.
                    // Construct a new delegate that does not have a target;
                    reference = new WeakReference(target);
                    var delegateType = typeof(Action<,,,>).MakeGenericType(target.GetType(), typeof(T[]), typeof(string), typeof(DateTime));
                    method = Delegate.CreateDelegate(delegateType, subscriber.Method);
                }
                else
                {
                    // It is a static method, so there is no associated target. 
                    // Hold a strong reference to the delegate.
                    reference = null;
                    method = subscriber;
                }
            }

            public object Target { get { return reference.Target; } }

            public System.Reflection.MethodInfo Method { get { return method.Method; } }

            public bool IsAlive
            {
                get
                {
                    // If the reference is null it was a Static method
                    // and therefore is always "Alive".
                    if (reference == null)
                    {
                        return true;
                    }
                    return reference.IsAlive;
                }
            }

            public bool Invoke(T[] e, string n, DateTime t)
            {
                object target = null;
                if (reference != null)
                {
                    target = reference.Target;
                }

                if (!IsAlive)
                {
                    return false;
                }

                if (target != null)
                {
                    method.DynamicInvoke(target, e, n, t);
                }
                else
                {
                    method.DynamicInvoke(e, n, t);
                }

                return true;
            }
        }

        private class Cache<T>
        {
            public List<T> List { get; private set; }
            public DateTime Updated { get; set; }

            public Cache(List<T> list = null)
            {
                if (list == null)
                {
                    list = new List<T>();
                }
                List = list;
                Updated = DateTime.UtcNow;
            }
        }

        #endregion

        #region Communication

        private string Wrap<T>(T data)
        {
            // Build request.
            var req = new DataWrapper<T, Empty> { Data = data, Extra = null };
            return JsonSerializer.Serialize(req, jsonSerializerOptions);
        }

        private string Wrap<T, K>(T data, K extra)
        {
            // Build request.
            var req = new DataWrapper<T, K> { Data = data, Extra = extra };
            return JsonSerializer.Serialize(req, jsonSerializerOptions);
        }

        private (T data, K extra) Unwrap<T, K>(string g, bool checkData = true)
        {
            // Parse response.
            var res = JsonSerializer.Deserialize<DataWrapper<T, K>>(g, jsonSerializerOptions);

            if (checkData && res.Data == null)
            {
                throw new ArgumentException("Data not found.");
            }

            return (res.Data, res.Extra);
        }

        private (string data, string cursor) Send(string method, string endpoint, string body = null, UserCache cache = null, string cursor = null, bool refresh = true)
        {
            var client = new HttpClient
            {
                Timeout = new TimeSpan(0, 0, 30)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(versionHeader));
            var req = new HttpRequestMessage { RequestUri = new Uri(apiUri + endpoint) };

            switch (method)
            {
                case "GET":
                    req.Method = HttpMethod.Get;
                    break;
                case "POST":
                    req.Method = HttpMethod.Post;
                    break;
                case "PUT":
                    req.Method = HttpMethod.Put;
                    break;
                case "DELETE":
                    req.Method = HttpMethod.Delete;
                    break;
                default:
                    throw new ArgumentException($"Unrecognized HTTP method ({method}).");
            }

            if (cursor != null)
            {
                if (cursor != "")
                {
                    req.Headers.Add("If-Range", cursor);
                }
                req.Headers.Range = new RangeHeaderValue(0, null)
                {
                    Unit = "items"
                };
            }

            if (cache?.accessToken != null)
            {
                req.Headers.Add("Authorization", "Bearer " + cache.accessToken);
            }

            if (method == "POST" || method == "PUT")
            {
                if (body == null)
                {
                    throw new Exception("Body cannot be null for POST or PUT requests.");
                }

                req.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            else if (body != null)
            {
                throw new IllegalArgumentException("Only POST and PUT requests can have a body.");
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Sending: " + method + " " + apiUri + endpoint + ", " + body + ", token: " + cache?.accessToken + ", cursor: " + cursor);
#endif

            var res = client.SendAsync(req).Result;
            client.Dispose();
            string g = res.Content.ReadAsStringAsync().Result;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("GOT from send: " + g + " (" + res.StatusCode + ")");
#endif

            if (res.IsSuccessStatusCode)
            {
                string from;
                if (cursor != null)
                {
                    if (res.Content.Headers.LastModified != null)
                    {
                        from = res.Content.Headers.LastModified.Value.UtcDateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);
                    }
                    else if (res.Headers.ETag != null)
                    {
                        from = res.Headers.ETag.Tag;
                    }
                    else
                    {
                        from = null;
                    }
                }
                else
                {
                    from = null;
                }

                return (g, from);
            }
            else
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Try to refresh token.
                    if (refresh && cache?.refreshToken != null)
                    {
                        // Write request body.
                        var request = new DataWrapper<string, Empty> { Data = cache.refreshToken };
                        var b = JsonSerializer.Serialize(request, jsonSerializerOptions);

                        // Send request.
                        (g, _) = Send("POST", "users/" + cache.userId + "/token/refresh", b);

                        // Parse response.
                        var response = JsonSerializer.Deserialize<DataWrapper<string, Empty>>(g, jsonSerializerOptions);

                        if (response.Data == null)
                        {
                            throw new ArgumentException("Data not found.");
                        }

                        // Update access token.
                        Settings.AccessToken = cache.accessToken = response.Data;

                        // Try again with refreshed token.
                        return Send(method, endpoint, body, cache, cursor, false);
                    }
                    else
                    {
                        throw new UnauthorizedApiRequestException();
                    }
                }
                else if (res.StatusCode == HttpStatusCode.UpgradeRequired)
                {
                    UpgradeRequiredEvent?.Invoke(); // Notify subscribers of update.
                    throw new UpgradeRequiredException();
                }
                else
                {
                    // Try to parse JSON body.
                    Fault f;
                    try
                    {
                        // Look for fault.
                        f = Fault.TryGetFault(g);
                    }
                    catch
                    {
                        f = null;
                    }

                    if (f != null)
                    {
                        throw f;
                    }
                    else
                    {
                        throw new Exception("Unsupported error: " + res.StatusCode);
                    }
                }
            }
        }
        private async Task<T> UploadVideo<T>(T o, string method, string url, Stream media, string ending, UserCache cache, bool refresh = true)
        {
            return await UploadMedia(o, method, url, media, ending, cache, "video", refresh);
        }

        private async Task<T> UploadMedia<T>(T o, string method, string url, Stream media, string ending, UserCache cache, string type = "image", bool refresh = true)
        {
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to delete.");
            }
            else if (o == null)
            {
                throw new IllegalArgumentException("Object cannot be null.");
            }
            else if (media == null)
            {
                throw new IllegalArgumentException(type + " cannot be null.");
            }

            HttpMethod m;
            if (method == "POST")
            {
                m = HttpMethod.Post;
            }
            else if (method == "PUT")
            {
                m = HttpMethod.Put;
            }
            else
            {
                throw new IllegalArgumentException("Cannot upload an " + type + " using " + method + " (only POST or PUT allowed).");
            }

            return await SystemTask.Run(async () =>
            {
                // Send request.
                var client = new HttpClient
                {
                    Timeout = new TimeSpan(0, 2, 0)
                };
                var req = new HttpRequestMessage(m, apiUri + url);
                req.Headers.Add("Authorization", "Bearer " + cache.accessToken);
                var form = new MultipartFormDataContent();
                var i = new StreamContent(media);
                i.Headers.Add("Content-Disposition", "form-data; name=\"" + type + "\"; filename=\"" + type + "." + ending + "\"");
                i.Headers.Add("Content-Type", type + "/" + ending);
                form.Add(i, type, type + "." + ending);
                req.Content = form;
                var res = await client.SendAsync(req);
                var g = await res.Content.ReadAsStringAsync();
                client.Dispose();

                if (res.IsSuccessStatusCode)
                {
                    // Parse response.
                    var r = JsonSerializer.Deserialize<DataWrapper<T, Empty>>(g, jsonSerializerOptions);

                    if (r.Data == null)
                    {
                        throw new ArgumentException("Data not found.");
                    }

                    return r.Data;
                }
                else
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Try to refresh token.
                        if (refresh && cache?.refreshToken != null)
                        {
                            // Write request body.
                            var request = new DataWrapper<string, Empty> { Data = cache.refreshToken };
                            var b = JsonSerializer.Serialize(request, jsonSerializerOptions);

                            // Send request.
                            (g, _) = Send("POST", "users/" + cache.userId + "/token/refresh", b);

                            // Parse response.
                            var response = JsonSerializer.Deserialize<DataWrapper<string, Empty>>(g, jsonSerializerOptions);
                            if (response.Data == null)
                            {
                                throw new ArgumentException("Data not found.");
                            }

                            // Update access token.
                            Settings.AccessToken = cache.accessToken = response.Data;

                            // Try again with refreshed token.
                            return await UploadMedia(o, method, url, media, ending, cache, type, false);
                        }
                        else
                        {
                            throw new UnauthorizedApiRequestException();
                        }
                    }
                    else if (res.StatusCode == HttpStatusCode.UpgradeRequired)
                    {
                        UpgradeRequiredEvent?.Invoke(); // Notify subscribers of update.
                        throw new UpgradeRequiredException();
                    }
                    else
                    {
                        // Try to parse JSON body.
                        Fault f;
                        try
                        {
                            // Look for fault.
                            f = Fault.TryGetFault(g);
                        }
                        catch
                        {
                            f = null;
                        }

                        if (f != null)
                        {
                            throw f;
                        }
                        else
                        {
                            throw new Exception("Unsupported error: " + res.StatusCode);
                        }
                    }
                }
            });
        }

        private async Task<T> UploadImage<T>(T o, string method, string url, Stream image, string ending, UserCache cache, bool refresh = true)
        {
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to delete.");
            }
            else if (o == null)
            {
                throw new IllegalArgumentException("Object cannot be null.");
            }
            else if (image == null)
            {
                throw new IllegalArgumentException("Image cannot be null.");
            }

            HttpMethod m;
            if (method == "POST")
            {
                m = HttpMethod.Post;
            }
            else if (method == "PUT")
            {
                m = HttpMethod.Put;
            }
            else
            {
                throw new IllegalArgumentException("Cannot upload an image using " + method + " (only POST or PUT allowed).");
            }

            return await SystemTask.Run(async () =>
            {
                // Send request.
                var client = new HttpClient
                {
                    Timeout = new TimeSpan(0, 2, 0)
                };
                var req = new HttpRequestMessage(m, apiUri + url);
                req.Headers.Add("Authorization", "Bearer " + cache.accessToken);
                var form = new MultipartFormDataContent();
                var i = new StreamContent(image);
                i.Headers.Add("Content-Disposition", "form-data; name=\"image\"; filename=\"image." + ending + "\"");
                i.Headers.Add("Content-Type", "image/" + ending);
                form.Add(i, "image", "image." + ending);
                req.Content = form;
                var res = await client.SendAsync(req);
                var g = await res.Content.ReadAsStringAsync();
                client.Dispose();

                if (res.IsSuccessStatusCode)
                {
                    // Parse response.
                    var r = JsonSerializer.Deserialize<DataWrapper<T, Empty>>(g, jsonSerializerOptions);

                    if (r.Data == null)
                    {
                        throw new ArgumentException("Data not found.");
                    }

                    return r.Data;
                }
                else
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Try to refresh token.
                        if (refresh && cache?.refreshToken != null)
                        {
                            // Write request body.
                            var request = new DataWrapper<string, Empty> { Data = cache.refreshToken };
                            var b = JsonSerializer.Serialize(request, jsonSerializerOptions);

                            // Send request.
                            (g, _) = Send("POST", "users/" + cache.userId + "/token/refresh", b);

                            // Parse response.
                            var response = JsonSerializer.Deserialize<DataWrapper<string, Empty>>(g, jsonSerializerOptions);
                            if (response.Data == null)
                            {
                                throw new ArgumentException("Data not found.");
                            }

                            // Update access token.
                            Settings.AccessToken = cache.accessToken = response.Data;

                            // Try again with refreshed token.
                            return await UploadImage(o, method, url, image, ending, cache, false);
                        }
                        else
                        {
                            throw new UnauthorizedApiRequestException();
                        }
                    }
                    else if (res.StatusCode == HttpStatusCode.UpgradeRequired)
                    {
                        UpgradeRequiredEvent?.Invoke(); // Notify subscribers of update.
                        throw new UpgradeRequiredException();
                    }
                    else
                    {
                        // Try to parse JSON body.
                        Fault f;
                        try
                        {
                            // Look for fault.
                            f = Fault.TryGetFault(g);
                        }
                        catch
                        {
                            f = null;
                        }

                        if (f != null)
                        {
                            throw f;
                        }
                        else
                        {
                            throw new Exception("Unsupported error: " + res.StatusCode);
                        }
                    }
                }
            });
        }

        private async Task<T> UploadPdf<T>(T o, string method, string url, Stream pdf, UserCache cache, bool refresh = true)
        {
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to delete.");
            }
            else if (o == null)
            {
                throw new IllegalArgumentException("Object cannot be null.");
            }
            else if (pdf == null)
            {
                throw new IllegalArgumentException("Pdf cannot be null.");
            }

            HttpMethod m;
            if (method == "POST")
            {
                m = HttpMethod.Post;
            }
            else if (method == "PUT")
            {
                m = HttpMethod.Put;
            }
            else
            {
                throw new IllegalArgumentException("Cannot upload a pdf using " + method + " (only POST or PUT allowed).");
            }

            return await SystemTask.Run(async () =>
            {
                // Send request.
                var client = new HttpClient
                {
                    Timeout = new TimeSpan(0, 2, 0)
                };
                var req = new HttpRequestMessage(m, apiUri + url);
                req.Headers.Add("Authorization", "Bearer " + cache.accessToken);
                var form = new MultipartFormDataContent();
                var i = new StreamContent(pdf);
                i.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"file.pdf\"");
                i.Headers.Add("Content-Type", "application/pdf");
                form.Add(i, "file", "file.pdf");
                req.Content = form;
                var res = await client.SendAsync(req);
                var g = await res.Content.ReadAsStringAsync();
                client.Dispose();

                if (res.IsSuccessStatusCode)
                {
                    // Parse response.
                    var r = JsonSerializer.Deserialize<DataWrapper<T, Empty>>(g, jsonSerializerOptions);

                    if (r.Data == null)
                    {
                        throw new ArgumentException("Data not found.");
                    }

                    return r.Data;
                }
                else
                {
                    if (res.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // Try to refresh token.
                        if (refresh && cache?.refreshToken != null)
                        {
                            // Write request body.
                            var request = new DataWrapper<string, Empty> { Data = cache.refreshToken };
                            var b = JsonSerializer.Serialize(request, jsonSerializerOptions);

                            // Send request.
                            (g, _) = Send("POST", "users/" + cache.userId + "/token/refresh", b);

                            // Parse response.
                            var response = JsonSerializer.Deserialize<DataWrapper<string, Empty>>(g, jsonSerializerOptions);
                            if (response.Data == null)
                            {
                                throw new ArgumentException("Data not found.");
                            }

                            // Update access token.
                            Settings.AccessToken = cache.accessToken = response.Data;

                            // Try again with refreshed token.
                            return await UploadPdf(o, method, url, pdf, cache, false);
                        }
                        else
                        {
                            throw new UnauthorizedApiRequestException();
                        }
                    }
                    else if (res.StatusCode == HttpStatusCode.UpgradeRequired)
                    {
                        UpgradeRequiredEvent?.Invoke(); // Notify subscribers of update.
                        throw new UpgradeRequiredException();
                    }
                    else
                    {
                        // Try to parse JSON body.
                        Fault f;
                        try
                        {
                            // Look for fault.
                            f = Fault.TryGetFault(g);
                        }
                        catch
                        {
                            f = null;
                        }

                        if (f != null)
                        {
                            throw f;
                        }
                        else
                        {
                            throw new Exception("Unsupported error: " + res.StatusCode);
                        }
                    }
                }
            });
        }



        //private async Task<bool> LazilyInvalidateEntity<T>(T entity) where T : IUpdate<T>
        //{
        //    return await Task.Run(() =>
        //    {
        //        string url;
        //        if (entity is Sender u)
        //        {
        //            url = "users/" + u.Id;
        //        }
        //        else
        //        {
        //            throw new UnsupportedOperationException("Unsupported type (" + entity.GetType() + ").");
        //        }

        //        return entity.Update(Send("GET", url).data);  // TODO: data wrapper.
        //    });
        //}

        #endregion

        #region Active

        public User ActiveUser
        {
            get
            {
                return userCache?.User;
            }
        }

        #endregion

        #region Sign up/in/out

        // Done when app starts up, if the user is already signed in.
        public void ImplicitSignIn()
        {
            LoadUserCache();
            StartPolling();
        }

        public async Task<bool> IsSignedIn()
        {
            // Is there a refresh token in settings? If not, user is not signed in.
            if (string.IsNullOrEmpty(Settings.RefreshToken))
            {
                return false;
            }

            // Wait for user poll to complete.
            await userPollComplete.Task; // NOTE: Does not use user cache.

            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Sender cache is null after successful user poll complete.");
                // TODO: Sign out instead?
            }
            else if (cache.User == null)
            {

                throw new IllegalStateException("Sender cache user is null after successful user poll complete.");
            }

            return true;
        }

        public void SignOut()
        {
            userPollComplete = new TaskCompletionSource<bool>(); // Ready for next user to sign in.
            userPollCancellationTokenSource?.Cancel(); // Stop polling thread.
            Settings.AccessToken = null;
            Settings.RefreshToken = null;
            Settings.UserId = null;
            Settings.ActiveOffice = null;
            userCache = null;
            //#if DEBUG
            DeleteEntities("user");
            DeleteEntities("user_cursor");
            DeleteEntities("bid");
            DeleteEntities("chat");
            DeleteEntities("craftsman");
            DeleteEntities("message");
            DeleteEntities("office");
            DeleteEntities("payment");
            DeleteEntities("rating");
            DeleteEntities("task");
            DeleteEntities("ad");
            //#endif
        }

        private class SigninResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string UserId { get; set; }
        }

        public async Task<string> SigninEmail(string email, string password)
        {
            return await SystemTask.Run(async () =>
            {
                try
                {
                    var body = Wrap(email, password);
                    var (g, _) = Send("POST", "users/signin", body);
                    var (data, _) = Unwrap<SigninResponse, Empty>(g);

                    // Set new user as active.
                    Settings.AccessToken = data.AccessToken;
                    Settings.RefreshToken = data.RefreshToken;
                    Settings.UserId = data.UserId;
                    Settings.ActiveOffice = null;

                    LoadUserCache(); // Load data into user cache, based on new active user.
                    StartPolling(); // Trigger a user poll now that the user has signed in.
                    await userPollComplete.Task; // Wait for user poll to complete. NOTE: Does not use user cache.

                    return data.UserId;
                }
                catch (Fault f)
                {
                    if (f.Code == FaultCode.WrongPassword || f.Code == FaultCode.NotFound)
                    {
                        throw new WrongCredentialsException(); // Rethrow as different type.
                    }

                    throw f;
                }
            });
        }

        private class SignupType
        {
            public string Password { get; set; }
            public string OpaqueNid { get; set; }
        }

        private class SignupExtra
        {
            public SignupType Type { get; set; }
            public LocationHelper.GeometryPoint Location { get; set; }
        }

        public async Task<User> Signup(User user, string opaqueNid, string password = null, LocationHelper.GeometryPoint location = null)
        {
            return await SystemTask.Run(async () =>
            {
                SignupType type = null;
                if (password == null)
                {
                    type = new SignupType
                    {
                        OpaqueNid = opaqueNid,
                    };
                }
                else
                {
                    type = new SignupType
                    {
                        Password = password,
                    };
                }

                var extra = new SignupExtra
                {
                    Location = location ?? new LocationHelper.GeometryPoint(),
                    Type = type,
                };

                var body = Wrap(user, extra);
                var (g, _) = Send("POST", "users", body);
                var (data, _) = Unwrap<SigninResponse, Empty>(g);

                // Set new user as active.
                Settings.AccessToken = data.AccessToken;
                Settings.RefreshToken = data.RefreshToken;
                Settings.UserId = data.UserId;

                LoadUserCache(); // Load data into user cache, based on new active user.
                StartPolling(); // Trigger a user poll now that the user has signed in.
                await userPollComplete.Task; // Wait for first user poll to complete.

                return userCache?.User; // First poll will have gotten the user
            });
        }

        #endregion

        #region Device

        public async SystemTask RegisterDevice(string pnsHandle)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Cannot register device without being signed in.");
            }
            var u = cache.User;

            await SystemTask.Run(() =>
            {
                var d = new User.Device { Handle = pnsHandle, AppId = AppInfo.PackageName, Build = int.Parse(AppInfo.BuildString), OS = DeviceInfo.Platform.ToString(), OSVer = DeviceInfo.VersionString };

                // Does this entry already exist?
                var update = true;
                if (u.Devices != null)
                {
                    for (var i = 0; i < u.Devices.Count; i++)
                    {
                        if (u.Devices[i].Handle == d.Handle && u.Devices[i].AppId == d.AppId && u.Devices[i].Build == d.Build && u.Devices[i].OS == d.OS && u.Devices[i].OSVer == d.OSVer)
                        {
                            update = false;
                            break;
                        }
                    }
                }
                if (update)
                {
                    var body = Wrap(d);

                    // Send request.
                    var (g, _) = Send("POST", "users/" + cache.userId + "/devices", body, cache);
                    var (data, _) = Unwrap<User, Empty>(g);

                    cache.User.Update(data);
                    PutEntity(cache.User, cache); // Store in db.
                }
            });
        }

        #endregion

        #region Subcription

        private static void Subscribe<T>(Action<T[], string, DateTime> success, Action<Exception, string> failure, List<Subscriber<T>> subscribers, string nonce, IPoll cache, Cache<T> entities, bool once)
        {
            SystemTask.Run(async () =>
            {
                if (cache == null)
                {
                    failure(new IllegalStateException("Cache not present."), nonce);
                    return;
                }

                await cache.Polled().Task; // Potentially wait for first poll to be complete.

                if (!once)
                {
                    var r = new Subscriber<T>(success);

                    // Subscribe to updates.
                    lock (subscribers)
                    {
                        // Remove any null references.
                        for (var i = subscribers.Count - 1; i >= 0; i--)
                        {
                            if (!subscribers[i].IsAlive)
                            {
                                subscribers.RemoveAt(i);
                            }
                        }

                        subscribers.Add(r);
                    }
                }

                // Return current cache objects.
                success(entities.List.ToArray(), nonce, entities.Updated); // NOTE: New thread not needed, no more work is to be done in this one.
            });
        }

        private static void Unsubscribe<T>(Action<T[], string, DateTime> subscriber, Action<Exception, string> failure, List<Subscriber<T>> subscribers, String nonce)
        {
            var found = false;
            lock (subscribers)
            {
                for (var i = subscribers.Count - 1; i >= 0; i--)
                {
                    if (!subscribers[i].IsAlive)
                    {
                        subscribers.RemoveAt(i);
                    }
                    else if (subscribers[i].Target == subscriber.Target && subscribers[i].Method == subscriber.Method)
                    {
                        subscribers.RemoveAt(i);
                        found = true;
                    }
                }
            }
            if (!found)
            {
                var e = new EntityNotFoundException("Could not find subscriber.");
                if (failure == null)
                {
                    throw e;
                }
                else
                {
                    failure(e, nonce);
                }
            }
        }

        public void Subscribe(Action<Bid[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, bidsSubscribers, nonce, cache, cache?.Bids, false);
        }

        public void Unsubscribe(Action<Bid[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, bidsSubscribers, nonce);
        }

        public void Subscribe(Action<Chat[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, chatsSubscribers, nonce, cache, cache?.Chats, false);
        }

        public void Unsubscribe(Action<Chat[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, chatsSubscribers, nonce);
        }
        public void Subscribe(Action<Message[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, messagesSubscribers, nonce, cache, cache?.Messages, false);
        }

        public void Unsubscribe(Action<Message[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, messagesSubscribers, nonce);
        }

        public void Subscribe(Action<Office[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, officesSubscribers, nonce, cache, cache?.Offices, false);
        }

        public void Unsubscribe(Action<Office[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, officesSubscribers, nonce);
        }

        public void Subscribe(Action<Payment[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, paymentsSubscribers, nonce, cache, cache?.Payments, false);
        }

        public void Unsubscribe(Action<Payment[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, paymentsSubscribers, nonce);
        }

        public void Subscribe(Action<Rating[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, ratingsSubscribers, nonce, cache, cache?.Ratings, false);
        }

        public void Unsubscribe(Action<Rating[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, ratingsSubscribers, nonce);
        }

        public void Subscribe(Action<Task[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, tasksSubscribers, nonce, cache, cache?.Tasks, false);
        }

        public void Unsubscribe(Action<Task[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, tasksSubscribers, nonce);
        }

        public void Subscribe(Action<Craftsman[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, craftsmenSubscribers, nonce, cache, cache?.Craftsmen, false);
        }

        public void Unsubscribe(Action<Craftsman[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, craftsmenSubscribers, nonce);
        }

        public void Subscribe(Action<Ad[], string, DateTime> success, Action<Exception, string> failure, string nonce = null)
        {
            var cache = userCache;
            Subscribe(success, failure, adsSubscribers, nonce, cache, cache?.Ads, false);
        }
        public void Unsubscribe(Action<Ad[], string, DateTime> subscriber, Action<Exception, string> failure = null, string nonce = null)
        {
            Unsubscribe(subscriber, failure, adsSubscribers, nonce);
        }
        #endregion

        #region Password

        public async SystemTask ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email must be present.");
            }

            await SystemTask.Run(() =>
            {
                var body = Wrap(email);
                Send("POST", "users/password/forgot", body);
            });
        }

        public async SystemTask ChangePassword(string oldPassword, string newPassword)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to change password.");
            }
            else if (string.IsNullOrWhiteSpace(oldPassword))
            {
                throw new ArgumentNullException("Old password cannot be null.");
            }
            else if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentNullException("New password cannot be null.");
            }

            await SystemTask.Run(() =>
            {
                var body = Wrap(newPassword, oldPassword);
                Send("PUT", "users/" + cache.userId + "/password", body, cache);
            });
        }

        #endregion

        #region Get
        private async Task<T> Get<T>(string table, string collection, string id, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, string grandParentCollection = null, string grandParentId = null, string grandGrandParentCollection = null, string grandGrandParentId = null, bool sort = false) where T : class, IUpdate<T>
        {
            var url = (grandParentCollection != null ? grandParentCollection + "/" : "") + (grandParentId != null ? grandParentId + "/" : "") + (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id;
            return await Get(table, id, cache, ucache, url, subscribers, sort);
        }

        private async Task<T> Get<T>(string table, string id, Cache<T> cache, UserCache ucache, string url, List<Subscriber<T>> subscribers = null, bool sort = false) where T : class, IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to get.");
            }
            else if (id == null)
            {
                throw new ArgumentNullException("Id cannot be null.");
            }

            // Try to get from cache.
            var o = GetEntityFromCache(id, cache);
            if (o != null)
            {
                return o;
            }

            // Try to get from db.
            o = GetEntity<T>(table, id);
            if (o != null)
            {
                // Store in cache and db.
                AddToCache(ref o, cache, ucache, subscribers, sort, null, false); // Update cache.

                return o;
            }

            // Get entity from api.
            return await SystemTask.Run(() =>
            {
                // Send request.
                var (g, _) = Send("GET", url, null, ucache);
                (o, _) = Unwrap<T, Empty>(g);

                // Store in cache and db.
                AddToCache(ref o, cache, ucache, subscribers, sort); // Update cache.

                return o;
            });
        }

        public async Task<User> GetUser(string id)
        {
            var cache = userCache;
            return await Get("user", "users", id, cache?.Users, cache);
        }

        public async Task<Office> GetOffice(string officeId, string id)
        {
            var cache = userCache;
            return await Get("office", "offices", id, cache?.Offices, cache, officesSubscribers);
        }

        public async Task<Payment> GetPayment(string officeId, string taskId, string bidId, string id)
        {
            var cache = userCache;
            var url = "offices/" + officeId + "/tasks/" + taskId + "/bids/" + bidId + "/payments/" + id;
            return await Get("payment", id, cache?.Payments, cache, url, paymentsSubscribers);
        }

        public async Task<Craftsman> GetCraftsman(string officeId, string id)
        {
            var cache = userCache;
            return await Get("craftsman", "craftsmen", id, cache?.Craftsmen, cache, craftsmenSubscribers, "offices", officeId);
        }

        public async Task<Task> GetTask(string officeId, string id)
        {
            var cache = userCache;
            return await Get("task", "tasks", id, cache?.Tasks, cache, tasksSubscribers, "offices", officeId);
        }

        public async Task<Bid> GetBid(string officeId, string taskId, string id)
        {
            var cache = userCache;
            return await Get("bid", "bids", id, cache?.Bids, cache, bidsSubscribers, "tasks", taskId, "offices", officeId);
        }

        #endregion

        #region Set
        // This method is a convenience in order to let the caller specify the url instead of requiring the grandparent argumentation
        // The caller is responsible for adding the /status to the end
        private async Task<T> SetPublishStatus<T>(PublishStatus status, string id, Cache<T> cache, UserCache ucache, string url, List<Subscriber<T>> subscribers = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to add.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(status);
                var (g, _) = Send("PUT", url, body, ucache);
                var (o, _) = Unwrap<T, Empty>(g);

                AddToCache(ref o, cache, ucache, subscribers, sort); // Update cache.

                return o;
            });
        }

        private async Task<T> SetPublishStatus<T>(PublishStatus status, string collection, string id, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, string grandParentCollection = null, string grandParentId = null, bool sort = false) where T : IUpdate<T>
        {
            var url = (grandParentCollection != null ? grandParentCollection + "/" : "") + (grandParentId != null ? grandParentId + "/" : "") + (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id + "/status";
            return await SetPublishStatus(status, id, cache, ucache, url, subscribers, sort);
        }

        #endregion

        #region Delete

        private async Task<T> Delete<T>(T o, string collection, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, string grandParentCollection = null, string grandParentId = null, string grandGrandParentCollection = null, string grandGrandParentId = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to delete.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }
            else if (o.Id == null)
            {
                throw new ArgumentNullException("Id cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var (g, _) = Send("DELETE", (grandGrandParentCollection != null ? grandGrandParentCollection + "/" : "") + (grandGrandParentId != null ? grandGrandParentId + "/" : "") + (grandParentCollection != null ? grandParentCollection + "/" : "") + (grandParentId != null ? grandParentId + "/" : "") + (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + o.Id, null, ucache);
                var (data, _) = Unwrap<T, Empty>(g);

                o.Update(data); // Update the object.
                AddToCache(new List<T> { o }, cache, ucache, subscribers, sort); // Update cache.

                return o;
            });
        }

        public async SystemTask Delete(User u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to delete account.");
            }
            else if (u.Id != cache.userId)
            {
                throw new ArgumentException("Cannot delete other user's account.");
            }

            await SystemTask.Run(() =>
            {
                Send("DELETE", "users/" + cache.userId, null, cache);
                SignOut();
            });
        }
        public async SystemTask Delete(Task t)
        {
            await Delete(t, "tasks", userCache?.Tasks, userCache, tasksSubscribers, "offices", t.OfficeId, null, null, null, null, true);
        }

        public async SystemTask Delete(Bid b)
        {
            await Delete(b, "bids", userCache?.Bids, userCache, bidsSubscribers, "tasks", b.TaskId, "offices", b.OfficeId, null, null, true);
        }

        public async SystemTask Delete(Chat c)
        {
            await Delete(c, "chats", userCache?.Chats, userCache, chatsSubscribers, "bids", c.BidId, "tasks", c.TaskId, "offices", c.OfficeId, true);
        }

        #endregion

        #region Update
        private async Task<T> Update<T>(T o, string collection, string id, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(o);
                var (g, _) = Send("PUT", (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id, body, ucache);
                var (data, _) = Unwrap<T, Empty>(g);

                o.Update(data); // Update the object.
                AddToCache(new List<T> { o }, cache, ucache, subscribers, sort); // Update cache.

                return o;
            });
        }

        public async Task<User> Update(User u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Sender cannot be null.");
            }
            else if (u.Id != cache.userId)
            {
                throw new ArgumentNullException("Cannot update other users.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "users/" + u.Id, body, cache);
                var (data, _) = Unwrap<User, Empty>(g);

                cache.User.Update(data); // Ignored if older.
                PutEntity(data, cache); // Store in db.

                return cache.User;
            });
        }

        public async Task<Task> Update(Task u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Task cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "offices/" + u.OfficeId + "/tasks/" + u.Id, body, cache);
                var (data, _) = Unwrap<Task, Empty>(g);
                u.Update(data);
                AddToCache(ref u, userCache.Tasks, cache, tasksSubscribers);
                return u;
            });
        }
        public async Task<Bid> Update(Bid u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Bid cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "offices/" + u.OfficeId + "/tasks/" + u.TaskId + "/bids/" + u.Id, body, cache);
                var (data, _) = Unwrap<Bid, Empty>(g);
                u.Update(data);
                AddToCache(ref u, userCache.Bids, cache, bidsSubscribers);
                return u;
            });
        }

        public async Task<Craftsman> Update(Craftsman u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Craftsman cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "offices/" + u.OfficeId + "/craftsmen/" + u.Id, body, cache);
                var (data, _) = Unwrap<Craftsman, Empty>(g);
                u.Update(data);
                AddToCache(ref u, userCache.Craftsmen, cache, craftsmenSubscribers);
                return u;
            });
        }

        public async Task<Message> Update(Message u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Message cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "offices/" + u.OfficeId + "/tasks/" + u.TaskId + "/bids/" + u.BidId + "/chats/" + u.ChatId + "/messages/" + u.Id, body, cache);
                var (data, _) = Unwrap<Message, Empty>(g);
                u.Update(data);
                AddToCache(ref u, userCache.Messages, cache, messagesSubscribers);
                return u;
            });
        }
        public async Task<List<Message>> MarkRead(Chat u)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to update.");
            }
            else if (u == null)
            {
                throw new ArgumentNullException("Message cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(u);
                var (g, _) = Send("PUT", "offices/" + u.OfficeId + "/tasks/" + u.TaskId + "/bids/" + u.BidId + "/chats/" + u.Id + "/read", body, cache);
                var (data, _) = Unwrap<List<Message>, Empty>(g);
                AddToCache(data, userCache.Messages, cache, messagesSubscribers, true); // Update cache.
                return data;
            });
        }

        #endregion

        #region Add
        private async Task<T> Add<T>(T o, string collection, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, string grandParentCollection = null, string grandParentId = null, string grandGrandParentCollection = null, string grandGrandParentId = null, bool sort = false) where T : IUpdate<T>
        {
            var url = (grandGrandParentCollection != null ? grandGrandParentCollection + "/" : "") + (grandGrandParentId != null ? grandGrandParentId + "/" : "") + (grandParentCollection != null ? grandParentCollection + "/" : "") + (grandParentId != null ? grandParentId + "/" : "") + (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection;
            return await Add(o, cache, ucache, url, subscribers, sort);
        }

        // Convenience method where you do not need to specify a bunch of grandparents here but instead can just specify a url
        private async Task<T> Add<T>(T o, Cache<T> cache, UserCache ucache, string url, List<Subscriber<T>> subscribers = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to add.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var body = Wrap(o);
                var (g, _) = Send("POST", url, body, ucache);
                var (data, _) = Unwrap<T, Empty>(g);

                o.Update(data); // Update the object.
                AddToCache(ref o, cache, ucache, subscribers, sort); // Update cache.

                return o;
            });
        }

        private async Task<T> AddImage<T>(T o, string method, string collection, string id, string end, Stream image, string ending, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, bool sort = false) where T : IUpdate<T>
        {
            var url = (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id + "/" + end;
            return await AddImage(o, method, id, image, ending, cache, ucache, url, subscribers, sort);
        }

        private async Task<T> AddImage<T>(T o, string method, string id, Stream image, string ending, Cache<T> cache, UserCache ucache, string url, List<Subscriber<T>> subscribers = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to add image.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }
            else if (id == null)
            {
                throw new ArgumentNullException("Id cannot be null.");
            }

            var res = await UploadImage(o, method, url, image, ending, ucache);

            await SystemTask.Run(() =>
            {
                o.Update(res); // Update the object.
                AddToCache(new List<T> { o }, cache, ucache, subscribers, sort); // Update cache.
            });

            return o;
        }

        private async Task<T> AddVideo<T>(T o, string method, string collection, string id, string end, Stream video, string ending, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to add video.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }
            else if (id == null)
            {
                throw new ArgumentNullException("Id cannot be null.");
            }
            // TODO(Jonathan): Currently we use the UploadImage function to upload the video, this should either be renamed to something like
            // UploadMedia or we should create a separate function for UploadVideo
            var res = await UploadVideo(o, method, (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id + "/" + end, video, ending, ucache);

            await SystemTask.Run(() =>
            {
                o.Update(res); // Update the object.
                AddToCache(new List<T> { o }, cache, ucache, subscribers, sort); // Update cache.
            });

            return o;
        }

        private async Task<T> AddBlob<T>(T o, string method, string collection, string id, string end, Stream blob, string ending, Cache<T> cache, UserCache ucache, List<Subscriber<T>> subscribers = null, string parentCollection = null, string parentId = null, bool sort = false) where T : IUpdate<T>
        {
            if (ucache == null)
            {
                throw new IllegalStateException("Must be signed in to add video.");
            }
            else if (cache == null)
            {
                throw new ArgumentNullException("Cache cannot be null.");
            }
            else if (o == null)
            {
                throw new ArgumentNullException("Object cannot be null.");
            }
            else if (id == null)
            {
                throw new ArgumentNullException("Id cannot be null.");
            }
            // TODO(Jonathan): Currently we use the UploadImage function to upload the blob, this should either be renamed to something like
            // UploadBlob or we should create a separate function for UploadBlob
            var res = await UploadImage(o, method, (parentCollection != null ? parentCollection + "/" : "") + (parentId != null ? parentId + "/" : "") + collection + "/" + id + "/" + end, blob, ending, ucache);

            await SystemTask.Run(() =>
            {
                o.Update(res); // Update the object.
                AddToCache(new List<T> { o }, cache, ucache, subscribers, sort); // Update cache.
            });

            return o;
        }

        public async Task<User> AddImage(User u, Stream image, string ending)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add image.");
            }

            return await SystemTask.Run(async () =>
            {
                var res = await UploadImage(u, "PUT", "users/" + u?.Id + "/image", image, ending, cache);
                u.Update(res);
                cache.User.Update(u); // Ignored if older.
                PutEntity(u, cache); // Store in db.
                return u;
            });
        }

        public async Task<Message> AddImage(Message u, Stream image, string ending)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add image.");
            }

            return await SystemTask.Run(async () =>
            {
                var res = await UploadImage(u, "PUT", "offices/" + u?.OfficeId + "/tasks/" + u?.TaskId + "/bids/" + u?.BidId + "/chats/" + u?.ChatId + "/messages/" + u?.Id + "/image", image, ending, cache);
                u.Update(res);
                AddToCache(new List<Message> { u }, cache.Messages, userCache, messagesSubscribers); // Update cache.
                return u;
            });
        }

        //public async Task<Post> AddImage(Post p, Stream image, string ending)
        //{
        //    return await AddImage(p, "PUT", "posts", p?.Id, "image", image, ending, userCache?.Posts, userCache, postsSubscribers, "areas", p.AreaId, true);
        //}

        //public async Task<Story> AddImage(Story s, Stream image, string ending)
        //{
        //    return await AddImage(s, "PUT", "stories", s?.Id, "image", image, ending, userCache?.Stories, userCache, storiesSubscribers, "areas", s.AreaId, true);
        //}

        public async Task<Task> Add(Task p, (Stream, string)[] images = null, (Stream, string)[] videos = null)
        {
            if (p.Images != null && p.Images.Length > 0)
            {
                throw new ArgumentException("New task cannot have images.");
            }
            if (p.Videos != null && p.Videos.Length > 0)
            {
                throw new ArgumentException("New task cannot have videos.");
            }
            else if (p.PublishStatus == PublishStatus.Flagged)
            {
                throw new ArgumentException("New post cannot be flagged.");
            }

            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add task.");
            }

            var stagedPublishing = (p.PublishStatus == PublishStatus.Published && ((images != null && images.Length > 0) || (videos != null && videos.Length > 0)));
            if (stagedPublishing)
            {
                p.PublishStatus = PublishStatus.Unpublished;
            }

            p = await Add(p, "tasks", cache?.Tasks, cache, tasksSubscribers, "offices", p.OfficeId);

            if (images != null && images.Length > 0)
            {
                foreach((var image, var ending) in images)
                {
                    p = await AddImage(p, "PUT", "tasks", p.Id, "image", image, ending, cache?.Tasks, cache, tasksSubscribers, "offices", p.OfficeId);
                }
            }

            if (videos != null && videos.Length > 0)
            {
                foreach ((var video, var ending) in videos)
                {
                    p = await AddVideo(p, "PUT", "tasks", p.Id, "video", video, ending, cache?.Tasks, cache, tasksSubscribers, "offices", p.OfficeId);
                }
            }

            if (stagedPublishing)
            {
                p = await SetPublishStatus(PublishStatus.Published, "tasks", p.Id, cache?.Tasks, cache, tasksSubscribers, "offices", p.OfficeId);
            }


            return p;
        }

        public async Task<Bid> Add(Bid p)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add bid.");
            }

            p = await Add(p, "bids", cache?.Bids, cache, bidsSubscribers, "tasks", p.TaskId, "offices", p.OfficeId);

            return p;
        }

        public async Task<Craftsman> Add(Rating p)
        {
            var cache = userCache;
            if (cache.Craftsmen == null)
            {
                throw new IllegalStateException("Must be signed in to add rating.");
            }

            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add.");
            }
            else if (p == null)
            {
                throw new ArgumentNullException("Bid cannot be null.");
            }

            return await SystemTask.Run(() =>
            {
                var url = "offices/" + p.OfficeId + "/craftsmen/" + p.CraftsmanId + "/ratings";
                var body = Wrap(p);
                var (g, _) = Send("POST", url, body, cache);
                var (data, _) = Unwrap<Craftsman, Empty>(g);
                Craftsman o = null;
                foreach (var craftsman in cache.Craftsmen.List)
                {
                    if (craftsman.Id == p.CraftsmanId)
                    {
                        craftsman.Update(data); // Update the object
                        o = craftsman;
                    }
                }
                if (o == null)
                {
                    o = data;
                }
                AddToCache(ref o, cache.Craftsmen, cache, craftsmenSubscribers); // Update cache.

                return o;
            });
        }

        public async Task<Craftsman> Add(Craftsman p, Stream certificate = null, string ending = null)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to apply for being a craftsman.");
            }

            p = await Add(p, "craftsmen", cache?.Craftsmen, cache, craftsmenSubscribers, "offices", p.OfficeId);
            if (certificate != null && ending != null)
            {
                // TODO(Jonathan): Here we are uploading a certificate using the "uploadimage" function, not great
                p = await UploadImage(p, "PUT", "offices/" + p?.OfficeId + "/craftsmen/" + p?.Id + "/certificate", certificate, ending, cache);
            }

            return p;
        }

        public async Task<Message> Add(Message p, Stream image = null, string ending = null)
        {
            if (p.Image != null)
            {
                throw new ArgumentException("New message cannot have certificates.");
            }

            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to send a message.");
            }

            var stagedPublishing = (p.PublishStatus == PublishStatus.Published && (image != null));
            if (stagedPublishing)
            {
                p.PublishStatus = PublishStatus.Unpublished;
            }

            string baseUrl = "offices/" + p.OfficeId + "/tasks/" + p.TaskId + "/bids/" + p.BidId + "/chats/" + p.ChatId + "/messages/";
            p = await Add(p, cache?.Messages, cache, baseUrl, messagesSubscribers, true);

            if (image != null)
            {
                p = await AddImage(p, "PUT", p.Id, image, ending, cache?.Messages, cache, baseUrl + p.Id + "/image", messagesSubscribers);
                p = await SetPublishStatus(PublishStatus.Published, p.Id, cache?.Messages, cache, baseUrl + p.Id + "/status", messagesSubscribers);
            }

            return p;
        }


        //public async Task<PostMessage> Add(PostMessage m)
        //{
        //    if (m.Status == PublishStatus.Flagged)
        //    {
        //        throw new ArgumentException("New post messages cannot be flagged.");
        //    }

        //    return await Add(m, "messages", userCache?.PostMessages, userCache, postMessagesSubscribers, "posts", m.PostId, "areas", m.AreaId, true);
        //}

        #endregion

        #region TaskFinish
        public async SystemTask CraftsmanFinishTask(string officeId, string taskId)
        {
            await SystemTask.Run(() =>
            {
                var (g, _) = Send("PUT", "offices/" + officeId + "/tasks/" + taskId + "/craftsman_finish", "", userCache);
            });
        }

        public async SystemTask FinishTask(string officeId, string taskId)
        {
            await SystemTask.Run(() =>
            {
                var (g, _) = Send("PUT", "offices/" + officeId + "/tasks/" + taskId + "/finish", "", userCache);
            });
        }

        #endregion

        #region Craft
        private struct CraftApplyResponse
        {
            public Craftsman Craftsman { get; set; }
            public string CraftId { get; set; }
        }

        public async Task<(Craftsman, string)> CraftApply(string officeId, string craftsman_id, Craft craft)
        {
            return await SystemTask.Run(() =>
            {
                var body = Wrap(craft);
                var (g, _) = Send("POST", "offices/" + officeId + "/craftsmen/" + craftsman_id + "/crafts/", body, userCache);
                var (r, _) = Unwrap<CraftApplyResponse, Empty>(g);
                return (r.Craftsman, r.CraftId);
            });
        }

        public async Task<Craftsman> AddCertificate(Craftsman u, string craft_id, Stream certificate, string ending)
        {
            var cache = userCache;
            if (cache == null)
            {
                throw new IllegalStateException("Must be signed in to add a certificate.");
            }

            return await SystemTask.Run(async () =>
            {
                var res = await UploadImage(u, "PUT", "offices/" + u.OfficeId + "/craftsmen/" + u?.Id + "/crafts/" + craft_id + "/certificate", certificate, ending, cache);
                u.Update(res);
                AddToCache(new List<Craftsman> { u }, cache.Craftsmen, userCache, craftsmenSubscribers); // Update cache.
                return u;
            });
        }
        #endregion

        #region Swish
        private struct AcceptResponse {
            public string PaymentRequestToken { get; set; }
            public string PaymentId { get; set; }
        }

        public async Task<(string, string)> AcceptBid(string officeId, string taskId, string bidId)
        {
            return await SystemTask.Run(() =>
            {
                var (g, _) = Send("PUT", "offices/" + officeId + "/tasks/" + taskId + "/bids/" + bidId + "/accept", "", userCache);
                (AcceptResponse r, _) = Unwrap<AcceptResponse, Empty>(g);
                return (r.PaymentRequestToken, r.PaymentId);
            });
        }

        public async Task<PaymentState> WaitForPaymentChange(string officeId, string taskId, string bidId, string paymentId)
        {
            return await SystemTask.Run(async () =>
            {
                // MAGIC NUMBER 150 is the amount of loops before we've waited for 5 minutes. If we have waited for two
                // minutes but the state has not changed then we throw an exception.
                for (int i = 0; i < 150; i++)
                {
                    var payment = await GetPayment(officeId, taskId, bidId, paymentId);
                    if (payment.PaymentState == PaymentState.Initialized)
                    {
                        // MAGIC NUMBER is 2 seconds of wait between polling payment again
                        await SystemTask.Delay(2000);
                    }
                    else
                    {
                        return payment.PaymentState;
                    }
                }
                throw new Exception("Payment state has not changed for five minutes");
            });
        }

        #endregion

        #region BankID

        private class RequestBankIdAutostartTokenRequest
        {
            public string Pnum { get; set; }
        }

        private class BankIdTokenResponse
        {
            public string AutoStartToken { get; set; }

            public string OrderRef { get; set; }
        }

        public async Task<(string autoStartToken, string orderRef)> RequestBankIdAutostartToken(string nid = null)
        {
            var body = Wrap<Empty>(null);
            return await SystemTask.Run(() =>
            {
                (var g, _) = Send("POST", "users/bankid/se", body);
                var (data, _) = Unwrap<BankIdTokenResponse, Empty>(g);

                return (data.AutoStartToken, data.OrderRef);
            });
        }

        private class BankIdSigninResponse
        {
            public long RetryIn { get; set; }

            public string AccessToken {get; set;}

            public string RefreshToken {get; set;}

            public string UserId {get; set;}

            public string OpaqueNid {get; set;}

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public async Task<(string userId, string opaqueNid, string firstName, string lastName)> BankIdSignin(string orderRef)
        {
            var body = Wrap(orderRef);
            string userId = null, opaqueNid = null, firstName = null, lastName = null;
            await SystemTask.Run(async () =>
            {
                while (true)
                {
                    (var g, _) = Send("POST", "users/bankid/se", body);
                    var (data, _) = Unwrap<BankIdSigninResponse, Empty>(g);

                    // Still pending?
                    if (data.RetryIn > 0)
                    {
                        var s = new TimeSpan(data.RetryIn);
                        System.Diagnostics.Debug.WriteLine("Delay bankid poll for " + s);
                        await SystemTask.Delay(s);
                    }
                    else
                    {
                        if (data.OpaqueNid != null)
                        {
                            opaqueNid = data.OpaqueNid;
                            firstName = data.FirstName;
                            lastName = data.LastName;
                        }
                        else
                        {
                            // Set new user as active.
                            Settings.AccessToken = data.AccessToken;
                            Settings.RefreshToken = data.RefreshToken;
                            Settings.UserId = data.UserId;
                            Settings.ActiveOffice = null;

                            LoadUserCache(); // Load data into user cache, based on new active user.
                            StartPolling(); //Poll(); // Trigger a user poll now that the user has signed in.
                            await userPollComplete.Task; // Wait for user poll to complete. NOTE: Does not use user cache.

                            userId = data.UserId;
                        }

                        break;
                    }
                }
            });

            return (userId, opaqueNid, firstName, lastName);
        }

        #endregion
    }
}
