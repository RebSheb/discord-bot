using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace NewBotRate.Database
{
    public class Tag
    {
        [Key]
        public ulong TagID { get; set; }
        public ulong GuildID { get; set; }
        public ulong UserID { get; set; }
        public string TagName { get; set; }
        public string TagMsg { get; set; }
    }
}
