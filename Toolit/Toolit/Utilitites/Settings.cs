using System;
using System.Text.Json;
using System.Threading.Tasks;
using Toolit.Models.Misc;
using Xamarin.Essentials;
using Xamarin.Forms;
using SystemTask = System.Threading.Tasks.Task;

namespace Toolit
{
    public static class Settings
    {
        #region Secure storage access methods

        private static async Task<string> GetFromSecureStorage(string key)
        {
            try
            {
                var data = await SecureStorage.GetAsync(key);
#if DEBUG
                if (data == null)
                {
                    throw new NullReferenceException("Debug: Secure storage not available");
                }
#endif
                return data;
            }
            catch (Exception ex)
            {
                // use regular settings if secure storage is unavailable
                Console.WriteLine($"SecureStorage access error: {ex}: {ex.Message}\n");
                return Preferences.Get(key, string.Empty);
            }
        }

        private static async SystemTask SetFromSecureStorage(string key, string value)
        {
            try
            {
#if DEBUG
                if (Device.RuntimePlatform == Device.iOS)
                {
                    throw new NullReferenceException("Debug: Secure storage not available");
                }
#endif
                await SecureStorage.SetAsync(key, value);
            }
            catch (Exception ex)
            {
                // use regular settings if secure storage is unavailable
                Console.WriteLine($"SecureStorage access error: {ex}: {ex.Message}\n");
                Preferences.Set(key, value);
            }
        }

        #endregion

        public static string AccessToken
        {
            get { return Preferences.Get("access_token", null); }
            set { Preferences.Set("access_token", value); }
        }

        public static string RefreshToken
        {
            get { return Preferences.Get("refresh_token", null); }
            set { Preferences.Set("refresh_token", value); }
        }

        public static string UserId
        {
            get { return Preferences.Get("user_id", null); }
            set { Preferences.Set("user_id", value); }
        }

        public static string ActiveOffice
        {
            get { return Preferences.Get("active_office", null); }
            set { Preferences.Set("active_office", value); }
        }
        
        public static bool IsNotFirstLaunch
        {
            get { return Preferences.Get("is_not_first_launch", false); }
            set { Preferences.Set("is_not_first_launch", value); }
        }
        
        public static AddressSettingsModel SavedAddress
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Preferences.Get("saved_address", string.Empty)))
                {
                    return JsonSerializer.Deserialize<AddressSettingsModel>(Preferences.Get("saved_address", string.Empty));
                }

                return null;
            }
            set { Preferences.Set("saved_address", JsonSerializer.Serialize(value)); }
        }

        public static async Task<DateTime> GetSessionTimeoutTime()
        {
            var dtString = await GetFromSecureStorage("SessionTimeoutDatetime");

            return DateTime.TryParse(dtString, out DateTime dt) ? dt : DateTime.MinValue;
        }

        public static async SystemTask SetSessionTimeoutTime(DateTime dt)
        {
#if DEBUG
            Console.WriteLine($"Timeout set to {dt}");
#endif
            await SetFromSecureStorage("SessionTimeoutDatetime", dt.ToString("s"));
        }

        public static async Task<string> GetNotificationTokenAsync()
        {
            return await GetFromSecureStorage("pnsToken");
        }

        public static async SystemTask SetNotificationTokenAsync(string newValue)
        {
            await SetFromSecureStorage("pnsToken", newValue);
        }
    }
}