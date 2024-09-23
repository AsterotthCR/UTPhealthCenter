﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace HealthCare.ViewModel
{
    public class AppointmentDto
    {
        public int id { get; set; }
        public int patientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string description { get; set; }

        public string patients { get; set; }
    }

    public class PatientAppointmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AppointmentDto _newAppointment;
        public AppointmentDto NewAppointment
        {
            get => _newAppointment;
            set
            {
                _newAppointment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewAppointment)));
            }
        }

        private ObservableCollection<AppointmentDto> _pendingAppointments;
        public ObservableCollection<AppointmentDto> PendingAppointments
        {
            get => _pendingAppointments;
            set
            {
                _pendingAppointments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PendingAppointments)));
            }
        }

        private ObservableCollection<AppointmentDto> _scheduledAppointments;
        public ObservableCollection<AppointmentDto> ScheduledAppointments
        {
            get => _scheduledAppointments;
            set
            {
                _scheduledAppointments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScheduledAppointments)));
            }
        }

        public PatientAppointmentViewModel()
        {
            NewAppointment = new AppointmentDto();
            PendingAppointments = new ObservableCollection<AppointmentDto>();
            ScheduledAppointments = new ObservableCollection<AppointmentDto>();
            LoadAppointments();
        }

        public async Task LoadAppointments()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync("https://giowebtestapinstance.azurewebsites.net/api/Appointments");
                var appointments = JsonSerializer.Deserialize<List<AppointmentDto>>(response);

                PendingAppointments.Clear();
                ScheduledAppointments.Clear();

                foreach (var appointment in appointments)
                {
                    if (appointment.AppointmentDate >= DateTime.Now)
                    {
                        ScheduledAppointments.Add(appointment);
                    }
                    else
                    {
                        PendingAppointments.Add(appointment);
                    }
                }

                Console.WriteLine(appointments);
            }


        }

        public async Task SaveAppointment()
        {
            var appointment = new
            {
                patientId = NewAppointment.patientId,
                appointmentDate = NewAppointment.AppointmentDate,
                description = NewAppointment.description
            };

            using (var httpClient = new HttpClient())
            {
                var json = JsonSerializer.Serialize(appointment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://giowebtestapinstance.azurewebsites.net/api/Appointments", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadAppointments();
                    await Application.Current.MainPage.DisplayAlert("Success", "Appointment saved successfully", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to save appointment", "OK");

                }
            }
        }

        public async Task DeleteAppointment(AppointmentDto appointment)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync($"https://giowebtestapinstance.azurewebsites.net/api/Appointments/{appointment.id}");

                if (response.IsSuccessStatusCode)
                {
                    await LoadAppointments();
                    await Application.Current.MainPage.DisplayAlert("Success", "Appointment deleted successfully", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete appointment", "OK");
                }
            }
        }
    }
}
