using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Toolit
{
    public abstract class TracedException : Exception
    {
        protected string member;
        protected string file;
        protected int line;

        public TracedException(string message, string member, string file, int line) : base(message)
        {
            this.member = member;
            this.file = file;
            this.line = line;
        }

        public string Member { get { return member; } }
        public string File { get { return file; } }
        public int Line { get { return line; } }

        public override string ToString()
        {
            return Message + ", member: " + member + ", file: " + file + ",  line: " + line;
        }
    }

    public class AnnotatedException : TracedException
    {
        protected Exception e;

        public AnnotatedException(Exception e, string message, [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line)
        {
            this.e = e;
        }

        public Exception Exception { get { return e; } }

        public override string ToString()
        {
            return Message + ", member: " + member + ", file: " + file + ",  line: " + line + " [" + e.ToString() + "]";
        }
    }

    public class UnauthorizedApiRequestException : TracedException
    {
        public UnauthorizedApiRequestException(string message = "Api returned Unauthorized.", [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    public class EntityNotFoundException : TracedException
    {
        public EntityNotFoundException(string message, [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    public class UpgradeRequiredException : TracedException
    {
        public UpgradeRequiredException(string message = "Upgrade required.", [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    public class UnsupportedOperationException : TracedException
    {
        public UnsupportedOperationException(string message = "Unsupported operation.", [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    public class IllegalStateException : TracedException
    {
        public IllegalStateException(string message = "Illegal state.", [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    public class IllegalArgumentException : TracedException
    {
        public IllegalArgumentException(string message = "Illegal argument.", [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(message, member, file, line) { }
    }

    [DataContract]
    public class Fault : TracedException
    {
        [DataMember]
        public FaultCode Code { get; private set; }

        [DataMember]
        public Fault[] Set { get; private set; }

        [DataMember]
        public bool Aligns { get; private set; }

        public Fault(FaultCode code, string text = "Fault.", Fault[] set = null, bool aligns = false, [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0) : base(text, member, file, line)
        {
            Code = code;
            Set = set;
            Aligns = aligns;
        }

        [DataContract]
        private class Candidate
        {
            [DataMember]
            public Bearer fault;

            [DataContract]
            public class Bearer
            {
                [DataMember(Name = "code")]
                public FaultCode Code { get; set; }

                [DataMember(Name = "text")]
                public string Text { get; set; }

                [DataMember(Name = "set", EmitDefaultValue = false)]
                public Bearer[] Set { get; set; }

                [DataMember(Name = "aligns", EmitDefaultValue = false)]
                public bool Aligns { get; set; }
            }

            public Fault Convert(string member, string file, int line)
            {
                if (fault != null)
                {
                    // Make set, if any.
                    Fault[] makeSet(Bearer b)
                    {
                        if (b.Set == null)
                        {
                            return null;
                        }

                        var set = new Fault[b.Set.Length];
                        for (var i = 0; i < b.Set.Length; i++)
                        {
                            set[i] = new Fault(b.Set[i].Code, b.Set[i].Text, makeSet(b.Set[i]), b.Set[i].Aligns, member, file, line);
                        }
                        return set;
                    }

                    return new Fault(fault.Code, fault.Text, makeSet(fault), fault.Aligns, member, file, line);
                }
                else
                {
                    return null;
                }
            }
        }

        public static Fault TryGetFault(string g, [System.Runtime.CompilerServices.CallerMemberName] string member = "", [System.Runtime.CompilerServices.CallerFilePath] string file = "", [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            if (string.IsNullOrWhiteSpace(g))
            {
                return null;
            }

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(g));
            var c = new DataContractJsonSerializer(typeof(Candidate), new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK") }).ReadObject(ms) as Candidate;
            ms.Close();
            return c.Convert(member, file, line);
        }
    }

    public class WrongCredentialsException : Exception
    {
        public WrongCredentialsException() : base() { }
    }

    public class NoCoverageException : Exception
    {
        public NoCoverageException() : base() { }
    }
}
