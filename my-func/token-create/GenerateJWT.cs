using System;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System.Collections.Generic;


	public class GenerateJWT
	{
        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        private const string SECRET = "123456781234567812345678";

        public GenerateJWT()
		{
                _algorithm = new HMACSHA256Algorithm();
                _serializer = new JsonNetSerializer();
                _base64Encoder = new JwtBase64UrlEncoder();
                _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
            }
            public string IssuingJWT(string email, string role)
            {
        var unixExp = new DateTimeOffset(DateTime.UtcNow.AddDays(1)).ToUnixTimeSeconds();
        Dictionary<string, object> claims = new Dictionary<string, object> {
                // JSON representation of the user Reference with ID and display name
                {
                    "name",
                    email
                },
                {
                    "role",
                    role
                },
                {
                    "exp",
                    unixExp
                }
            };
            // I think my signature isn't being base64 encoded, algorithm matches jwt.io
                string token = _jwtEncoder.Encode(claims, SECRET);
                return token;
            }
        }
    
