﻿namespace luizalabs.desafio_tecnico.Models.Legacy
{
    public class LegacyOut
    {
        public Guid request_id { get; set; }
        public string file_name { get; set; }
        public int total_lines { get; set; }
        public string status { get; set; }
        public float processed_lines { get; set; }
    }
}
