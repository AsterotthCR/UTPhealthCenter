using HealthCare.ViewModel;

namespace HealthCare.Views
{
    public partial class CitasPage : ContentPage
    {
        private PatientAppointmentViewModel _viewModel;

        public CitasPage()
        {
            InitializeComponent();
            _viewModel = new PatientAppointmentViewModel();
            BindingContext = _viewModel;
        }

        private void ShowPendingCitas(object sender, EventArgs e)
        {
            PendingCitasSection.IsVisible = true;
            //ScheduledCitasSection.IsVisible = false;
            AppointmentFormSection.IsVisible = false;
        }

        private void ShowScheduledCitas(object sender, EventArgs e)
        {
            PendingCitasSection.IsVisible = false;
            //ScheduledCitasSection.IsVisible = true;
            AppointmentFormSection.IsVisible = false;
        }

        private void ShowAppointmentForm(object sender, EventArgs e)
        {
            PendingCitasSection.IsVisible = false;
            // ScheduledCitasSection.IsVisible = false;
            AppointmentFormSection.IsVisible = true;
        }

        private async void OnSaveAppointment(object sender, EventArgs e)
        {
            await _viewModel.SaveAppointment();
        }

        private async void OnDeleteAppointment(object sender, EventArgs e)
        {
            var button = sender as Button;
            var appointment = button?.BindingContext as AppointmentDto;

            if (appointment != null)
            {
                await _viewModel.DeleteAppointment(appointment);
            }
        }
        private void OnFramePointerEntered(object sender, PointerEventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                frame.BackgroundColor = Colors.LightGray; // Cambiar color al hacer hover
            }
        }

        private void OnFramePointerExited(object sender, PointerEventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                frame.BackgroundColor = Color.FromArgb("#E3F2FD"); // Volver al color original
            }
        }

        // Cambiar el color del botón al hacer hover
        private void OnButtonPointerEntered(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.BackgroundColor = Colors.DarkRed; // Cambiar color del botón al hacer hover
            }
        }

        private void OnButtonPointerExited(object sender, PointerEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.BackgroundColor = Color.FromArgb("#E53935"); // Volver al color original
            }
        }

    }
}
