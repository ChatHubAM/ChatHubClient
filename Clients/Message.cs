using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubWPF.Clients
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
    }
}
