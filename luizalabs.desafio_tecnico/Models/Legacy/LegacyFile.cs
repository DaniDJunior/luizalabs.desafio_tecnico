﻿using System.ComponentModel.DataAnnotations;
using System.Data;

namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyFile
    {
        [Key]
        public Guid request_id { get; set; }
        public string file_name { get; set; }
        public int total_lines { get; set; }
        public string status { get; set; }
        public virtual ICollection<LegacyData> Lines { get; set; }

        public LegacyFile()
        {
            file_name = string.Empty;
            status = string.Empty;
            Lines = new List<LegacyData>();
        }

    }
}
