using System;
using System.Collections.Generic;
using System.Linq;

namespace Q2.HealthSystem
{
    // a. Generic Repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new List<T>(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var match = items.FirstOrDefault(predicate);
            if (match is null) return false;
            items.Remove(match);
            return true;
        }
    }

    // b. Patient
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"#{Id} {Name}, {Age}, {Gender}";
    }

    // c. Prescription
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() => $"Rx#{Id} Patient:{PatientId} {MedicationName} ({DateIssued:d})";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "F"));
            _patientRepo.Add(new Patient(2, "Bob Jones", 45, "M"));
            _patientRepo.Add(new Patient(3, "Ama Mensah", 27, "F"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Lisinopril", DateTime.Today.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Paracetamol", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(105, 2, "Metformin", DateTime.Today.AddDays(-3)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("-- Patients --");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"-- Prescriptions for PatientID {id} --");
            foreach (var rx in GetPrescriptionsByPatientId(id))
                Console.WriteLine(rx);
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            Console.WriteLine();
            app.PrintPrescriptionsForPatient(2);
        }
    }
}