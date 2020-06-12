using System;

namespace apiservice.Model.Db
{
    public partial class Accesscode
    {
        public long Accesscodeid { get; set; }
        public Guid Session { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public string Phonenumber { get; set; }
        public string Accesscode1 { get; set; }

        public virtual Main SessionNavigation { get; set; }
    }
}