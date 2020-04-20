
using ANPaX.DesktopUI.Models;

using Caliburn.Micro;

namespace ANPaX.DesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private string _firstName = "ANPaX";
        private string _lastName;
        private PersonModel _selectedPerson;

        public ShellViewModel()
        {
            People.Add(new PersonModel { FirstName = "Valentin", LastName = "Baric" });
            People.Add(new PersonModel { FirstName = "Steffi", LastName = "Baric" });
            People.Add(new PersonModel { FirstName = "Madita", LastName = "Baric" });
        }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public BindableCollection<PersonModel> People { get; set; } = new BindableCollection<PersonModel>();

        public PersonModel SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                NotifyOfPropertyChange(() => SelectedPerson);
            }
        }

        /// <summary>
        /// This automatically disables the button if both names are null or empty.
        /// This seems magic because it is implicitely actived.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public bool CanClearText(string firstName, string lastName)
        {
            return !string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName);
        }

        public void ClearText(string firstName, string lastName)
        {
            FirstName = "";
            LastName = "";
        }

        public void LoadPageOne()
        {
            ActivateItem(new FirstChildViewModel());
        }

        public void LoadPageTwo()
        {
            ActivateItem(new SecondChildViewModel());
        }
    }
}
