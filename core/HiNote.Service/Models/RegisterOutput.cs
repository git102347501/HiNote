using System;

namespace HiNote.Service.Models
{
    public class RegisterOutput
    {
        public RegisterOutputDto error { get; set; }
    }

    public class RegisterOutputDto
    {
        public string message { get; set; }
    }
}