﻿using System;
using System.Collections.Generic;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageDecoder
    {
        readonly CoapMessageOptionFactory _optionFactory = new CoapMessageOptionFactory();

        // TODO: Consider creating "CoapMessageDecodeResult" which has Message (null or set) and "DecodeResult" as INTERFACE with all possible errors (VersionInvalidDecodeResult) etc.
        public CoapMessage Decode(ArraySegment<byte> buffer)
        {
            using (var reader = new CoapMessageReader(buffer))
            {
                var version = reader.ReadBits(2);
                if (version != 0x1)
                {
                    throw new CoAPProtocolViolationException("Version is not set to 1.");
                }

                var type = reader.ReadBits(2);
                if (type > 2)
                {
                    throw new CoAPProtocolViolationException("Type is invalid.");
                }

                var tokenLength = reader.ReadBits(4);

                var code = (byte)reader.ReadBits(8);
                var codeClass = (byte)(code >> 5);
                var codeDetails = (byte)(0x1F & code);

                var id = reader.ReadByte() << 8;
                id |= reader.ReadByte();

                byte[] token = null;
                if (tokenLength > 0)
                {
                    token = reader.ReadBytes(tokenLength);
                }

                var options = DecodeOptions(reader);

                var payload = reader.ReadToEnd();

                var message = new CoapMessage
                {
                    Type = (CoapMessageType)type,
                    Code = new CoapMessageCode(codeClass, codeDetails),
                    Id = (ushort)id,
                    Token = token,
                    Options = options,
                    Payload = payload
                };

                return message;
            }
        }

        List<CoapMessageOption> DecodeOptions(CoapMessageReader reader)
        {
            var options = new List<CoapMessageOption>();

            if (reader.EndOfStream)
            {
                return options;
            }

            var lastNumber = 0;
            while (!reader.EndOfStream)
            {
                var delta = reader.ReadBits(4);
                var length = reader.ReadBits(4);

                if ((byte)(delta << 4 | length) == 0xFF)
                {
                    // Payload marker.
                    break;
                }

                if (delta == 13)
                {
                    delta = reader.ReadBits(8) + 13;
                }
                else if (delta == 14)
                {
                    delta = reader.ReadBits(8) + 269;
                }

                if (length == 13)
                {
                    length = reader.ReadBits(8) + 13;
                }
                else if (length == 14)
                {
                    length = reader.ReadBits(8) + 269;
                }

                byte[] value = null;
                if (length > 0)
                {
                    value = reader.ReadBytes(length);
                }

                var number = lastNumber + delta;
                lastNumber = number;

                options.Add(CreateOption(number, value));
            }

            return options;
        }

        CoapMessageOption CreateOption(int number, byte[] value)
        {
            if (number == (int)CoapMessageOptionNumber.IfMatch)
            {
                return _optionFactory.CreateIfMatch(value);
            }

            if (number == (int)CoapMessageOptionNumber.UriHost)
            {
                return _optionFactory.CreateUriHost(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.ETag)
            {
                return _optionFactory.CreateETag(value);
            }

            if (number == (int)CoapMessageOptionNumber.IfNoneMatch)
            {
                return _optionFactory.CreateIfNoneMatch();
            }

            if (number == (int)CoapMessageOptionNumber.UriPort)
            {
                return _optionFactory.CreateUriPort(DecodeUintOptionValue(value));
            }

            if (number == (int)CoapMessageOptionNumber.LocationPath)
            {
                return _optionFactory.CreateLocationPath(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.UriPath)
            {
                return _optionFactory.CreateUriPath(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.ContentFormat)
            {
                return _optionFactory.CreateContentFormat((CoapMessageContentFormat)value[0]);
            }

            if (number == (int)CoapMessageOptionNumber.MaxAge)
            {
                return _optionFactory.CreateMaxAge(DecodeUintOptionValue(value));
            }

            if (number == (int)CoapMessageOptionNumber.UriQuery)
            {
                return _optionFactory.CreateUriQuery(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.Accept)
            {
                return _optionFactory.CreateAccept(DecodeUintOptionValue(value));
            }

            if (number == (int)CoapMessageOptionNumber.LocationQuery)
            {
                return _optionFactory.CreateLocationQuery(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.ProxyUri)
            {
                return _optionFactory.CreateProxyUri(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.ProxyScheme)
            {
                return _optionFactory.CreateProxyScheme(System.Text.Encoding.UTF8.GetString(value));
            }

            if (number == (int)CoapMessageOptionNumber.Size1)
            {
                return _optionFactory.CreateSize1(DecodeUintOptionValue(value));
            }

            throw new NotSupportedException();
        }

        uint DecodeUintOptionValue(byte[] value)
        {
            if (value.Length == 1)
            {
                return value[0];
            }

            if (value.Length == 2)
            {
                return (uint)(value[0] << 8 | value[1]);
            }

            if (value.Length == 3)
            {
                return (uint)(value[0] << 16 | value[1] << 8 | value[2]);
            }

            if (value.Length == 4)
            {
                return (uint)(value[0] << 24 | value[1] << 16 | value[2] << 8 | value[3]);
            }

            throw new CoAPProtocolViolationException("The buffer for the uint option is too long.");
        }
    }
}
