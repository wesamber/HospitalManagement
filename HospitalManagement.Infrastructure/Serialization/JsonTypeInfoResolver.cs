using HospitalManagement.Domain.Entities.Doctors;
using HospitalManagement.Domain.Entities.Patients;
using HospitalManagement.Domain.Entities.Treatments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Serialization;

public class JsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);


        if (type == typeof(Patient))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "type",
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(InternalPatient), "internal"),
                    new JsonDerivedType(typeof(ExternalPatient), "external")
                }
            };
        }

        if(type == typeof(Treatment))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "type",
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(TreatmentInternal), "internal"),
                    new JsonDerivedType(typeof(TreatmentExternal), "external")
                }
            };
        }

        if (type == typeof(DoctorRole))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "roleType",
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(PermanentRole), "permanent"),
                    new JsonDerivedType(typeof(TraineeRole), "trainee"),
                    new JsonDerivedType(typeof(ContractedRole), "contracted")
                }
            };
        }

        return jsonTypeInfo;
    }
}
