using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PollApp.Backend.Models
{
    public class PollOption
    {
        public Guid Id { get; set; }
        public Guid PollId { get; set; }
        public string OptionText { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public Poll? Poll { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
