using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<UserActivity> Activities { get; set; }
        public User(){Activities = new List<UserActivity>();}
    }

    public class CreateUser
    {
        [Required(ErrorMessage = "Must have a First name")]      //validation
        [MinLength(2, ErrorMessage = "Must be 2 char or long")]  //validation
        public string First_name { get; set; }

        [Required(ErrorMessage = "Must have a Last name")]      //validation
        [MinLength(2, ErrorMessage = "Must be 2 char or long")]  //validation
        public string Last_name { get; set; }

        [Required(ErrorMessage = "Must have a Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Require a password")]
        [MinLength(8, ErrorMessage = "Password needs to be 8 char or longer")]
        [RegularExpression(@"^(?=[^\d_].*?\d)\w(\w|[!@#$%]){7,20}", ErrorMessage = @"Password must have one capital, one special character and one numerical character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Require a confirmation password")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string C_password { get; set; }
    }

    public class LoginUser
    {
        [Required(ErrorMessage = "Must have a Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Require a password")]
        [MinLength(8, ErrorMessage = "Password needs to be 8 char or longer")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Must have a title")]
        [MinLength(3, ErrorMessage = "Must be 3 char or long")] 
        public string Title { get; set; }

        [Required(ErrorMessage = "Must have a date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]  //I have this but it doens't work to just show the DATE
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Must have a time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH-mm}", ApplyFormatInEditMode = true)] //I have this but it doens't work to just show the TIME
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "Must have a description")]
        [Range(1, 360, ErrorMessage = "Duration must be more than 1")] 
        public int Duration { get; set; }

        public string Metric { get; set; }

        [Required(ErrorMessage = "Must have a description")]
        [MinLength(10, ErrorMessage = "Must a description 10 char or longer")] 
        public string Description { get; set; }
       
        public int UserId { get; set; }
        public string CreaterName { get; set; }
        public List<UserActivity> Participants { get; set; }
        public Activity(){Participants = new List<UserActivity>();}
    }
    public class UserActivity
    {
        [Key]
        public int UserActivityId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Activity")]
        public int ActivityId { get; set; }
        public string Name { get; set; }

    }

    public class MessageBoard
    {
        [Key]
        public int MessageBoardId { get; set; }

        [ForeignKey("ActivityId")]
        public int ActivityId { get; set; }
        public List<Chat> Chats { get; set; }
        public MessageBoard(){Chats = new List<Chat>();}
    }


    public class Chat 
    {
        [Key]
        public int ChatId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        
        [ForeignKey("Messageboard")]    
        public int MessageBoardId { get; set; }
        public string Name { get; set; }
        [Required]
        [MinLength(1)]
        public string Message { get; set; }

    }
}