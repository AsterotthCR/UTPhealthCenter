using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace HealthCare.ViewModel
{
    public class AppointmentDto
    {
        public int id { get; set; }
        public int patientId { get; set; }
        public string patientName { get; set; }
        public string doctorName { get; set; }
        public int doctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string description { get; set; }
        public string type { get; set; }

    }
    public class DoctorsDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
    }
    public class PatientDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
    }
    public class PatientAppointmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        // New Appointment
        private AppointmentDto _newAppointment;
        public AppointmentDto NewAppointment
        {
            get => _newAppointment;
            set
            {
                _newAppointment = value;
                OnPropertyChanged(nameof(NewAppointment));
            }
        }



        // Appointments Collections
        private ObservableCollection<AppointmentDto> _pendingAppointments = new ObservableCollection<AppointmentDto>();
        public ObservableCollection<AppointmentDto> PendingAppointments
        {
            get => _pendingAppointments;
            set
            {
                _pendingAppointments = value;
                OnPropertyChanged(nameof(PendingAppointments));
            }
        }

        private ObservableCollection<AppointmentDto> _scheduledAppointments = new ObservableCollection<AppointmentDto>();
        public ObservableCollection<AppointmentDto> ScheduledAppointments
        {
            get => _scheduledAppointments;
            set
            {
                _scheduledAppointments = value;
                OnPropertyChanged(nameof(ScheduledAppointments));
            }
        }

        // Patients Collection
        private ObservableCollection<PatientDto> _patients = new ObservableCollection<PatientDto>();
        public ObservableCollection<PatientDto> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged(nameof(Patients));
            }
        }
        private PatientDto _selectedPatient;
        public PatientDto SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                NewAppointment.patientId = _selectedPatient?.id ?? 0; // Actualizar el patientId cuando cambie el paciente seleccionado
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPatient)));
            }
        }

        // Appointment Types
        private ObservableCollection<string> _appointmentTypes = new ObservableCollection<string> { "Urgente", "Especialidad", "General" };
        public ObservableCollection<string> AppointmentTypes
        {
            get => _appointmentTypes;
            set
            {
                _appointmentTypes = value;
                OnPropertyChanged(nameof(AppointmentTypes));
            }
        }

        public PatientAppointmentViewModel()
        {
            NewAppointment = new AppointmentDto();
            LoadPatients(); // Load patients when ViewModel is instantiated
            LoadAppointments(); // Load appointments when ViewModel is instantiated
        }

        // Notify property changes for data binding
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Load Appointments
        public async Task LoadAppointments()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync("https://giowebtestapinstance.azurewebsites.net/api/Appointments");
                    var appointments = JsonSerializer.Deserialize<List<AppointmentDto>>(response);
                    // 2. Cargar los pacientes desde el API
                    var patientsResponse = await httpClient.GetStringAsync("https://giowebtestapinstance.azurewebsites.net/api/Patients");
                    var patients = JsonSerializer.Deserialize<List<PatientDto>>(patientsResponse);

                    var doctorsResponse = await httpClient.GetStringAsync("https://giowebtestapinstance.azurewebsites.net/api/Doctors");
                    var doctors = JsonSerializer.Deserialize<List<DoctorsDto>>(doctorsResponse);


                    PendingAppointments.Clear();
                    ScheduledAppointments.Clear();

                    foreach (var appointment in appointments)
                    {

                        // Encontrar el paciente correspondiente
                        var patient = patients.FirstOrDefault(p => p.id == appointment.patientId);
                        var doctor = doctors.FirstOrDefault(d => d.id == appointment.doctorId);
                        if (patient != null)
                        {
                            // Asignar el nombre completo del paciente
                            appointment.patientName = $"{patient.name} {patient.lastName}";
                        }
                        if (doctor != null)
                        {
                            // Asignar el nombre completo del paciente
                            appointment.doctorName = $"{doctor.name} {doctor.lastName}";
                        }

                        // Clasificar la cita entre pendientes y programadas
                        if (appointment.AppointmentDate >= DateTime.Now)
                        {
                            ScheduledAppointments.Add(appointment);
                        }
                        else
                        {
                            PendingAppointments.Add(appointment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar citas: {ex.Message}", "OK");
            }
        }

        // Load Patients
        public async Task LoadPatients()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync("https://giowebtestapinstance.azurewebsites.net/api/Patients");
                    var patients = JsonSerializer.Deserialize<List<PatientDto>>(response);

                    Patients.Clear();
                    foreach (var patient in patients)
                    {
                        Patients.Add(patient);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar pacientes: {ex.Message}", "OK");
            }
        }

        // Delete Appointment
        public async Task DeleteAppointment(AppointmentDto appointment)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync($"https://giowebtestapinstance.azurewebsites.net/api/Appointments/{appointment.id}");

                    if (response.IsSuccessStatusCode)
                    {
                        PendingAppointments.Remove(appointment);
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Cita eliminada correctamente", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la cita", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al eliminar cita: {ex.Message}", "OK");
            }
        }

        // Save Appointment
        public async Task SaveAppointment()
        {
            try
            {
                var appointment = new
                {
                    patientId = NewAppointment.patientId,  // 
                    doctorId = NewAppointment.doctorId,    // Convertir a entero
                    appointmentDate = NewAppointment.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"), // Formato correcto
                    description = NewAppointment.description,
                    type = NewAppointment.type
                };

                using (var httpClient = new HttpClient())
                {
                    var json = JsonSerializer.Serialize(appointment);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://giowebtestapinstance.azurewebsites.net/api/Appointments", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Cita guardada correctamente", "OK");
                        await LoadAppointments(); // Recargar citas después de guardar
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar la cita: {errorMessage}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al guardar cita: {ex.Message}", "OK");
            }
        }


    }
}
