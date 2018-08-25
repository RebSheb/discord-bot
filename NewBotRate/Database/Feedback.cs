using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace NewBotRate.Database
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }
        public ulong UserID { get; set; }
        public ulong GuildID { get; set; }
        public string Message { get; set; }
        public int Read { get; set; }
        
    }
}
