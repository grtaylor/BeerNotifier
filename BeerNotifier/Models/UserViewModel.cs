using System.Web.Mvc;
using BeerNotifier.Controllers;

namespace BeerNotifier.Models
{
    public class UserViewModel
    {
        public Participant ParticipantDetails { get; set; }
        public SelectList Locations { get; set; }
    }
}