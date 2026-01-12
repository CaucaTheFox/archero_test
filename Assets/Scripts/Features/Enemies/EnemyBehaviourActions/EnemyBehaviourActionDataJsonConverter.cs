using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Features.Enemies
{
    public class EnemyBehaviourActionDataJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value,  JsonSerializer serializer)
        {
            
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            
            var jo = JObject.Load(reader);

            var token = jo["Type"];
            var propertyTypeString = token.Value<string>();
            Enum.TryParse<EnemyBehaviourActionType>(propertyTypeString, out var propertyType);
            EnemyBehaviourActionData item = propertyType switch
            {
                EnemyBehaviourActionType.Idle => new IdleEnemyBehaviourActionData(),
                EnemyBehaviourActionType.Movement => new MovementEnemyBehaviourActionData(),
                EnemyBehaviourActionType.Dash => new DashEnemyBehaviourActionData(),
                EnemyBehaviourActionType.Attack => new AttackEnemyBehaviourActionData(),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(EnemyBehaviourActionData).IsAssignableFrom(objectType);
        }
    }
}