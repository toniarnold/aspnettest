using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asplib.Model
{
    /// <summary>
    /// TABLE [dbo].[Main]
    /// </summary>
    public partial class Main
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long mainid { get; set; }

        [Key]
        public Guid session { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime changed { get; set; }

        public Guid clsid { get; set; }

        public byte[] main { get; set; }
    }
}