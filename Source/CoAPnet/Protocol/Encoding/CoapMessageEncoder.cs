﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CoAPnet.Protocol.Encoding
{
    public sealed class CoapMessageEncoder
    {
        public ArraySegment<byte> Encode(CoapMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            ThrowIfInvalid(message);

            using (var writer = new CoapMessageWriter())
            {
                writer.WriteBits(0x1, 2); // Version
                writer.WriteBits((int)message.Type, 2); // Type
                writer.WriteBits(message.Token?.Length ?? 0, 4); // Token length

                var code = message.Code.Detail | message.Code.Class << 3;
                writer.WriteBits(code, 8);

                writer.WriteBits((byte)(message.Id >> 8), 8); // MessageId MSB
                writer.WriteBits((byte)(message.Id), 8); // MessageId LSB

                if (message.Token != null)
                {
                    writer.WriteBytes(message.Token);
                }

                EncodeOptions(message.Options, writer);

                if (message.Payload.Count > 0)
                {
                    writer.WriteBits(0xFF, 8);// Payload Marker
                    writer.WriteBytes(message.Payload.ToArray());
                }

                return writer.ToArray();
            }
        }

        void EncodeOptions(IEnumerable<CoapMessageOption> options, CoapMessageWriter writer)
        {
            if (options == null)
            {
                return;
            }

            var previousOptionNumber = 0;
            foreach (var option in options.OrderBy(o => o.Number))
            {
                // As per RFC: Only the delta of the option number is stored.
                var delta = option.Number - previousOptionNumber;
                previousOptionNumber = option.Number;

                byte[] value;

                if (option.Value is CoapMessageOptionEmptyValue)
                {
                    value = new byte[0];
                }
                else if (option.Value is CoapMessageOptionUintValue uintValue)
                {
                    value = EncodeUintOptioNValue(uintValue.Value);
                }
                else if (option.Value is CoapMessageOptionStringValue stringValue)
                {
                    value = System.Text.Encoding.UTF8.GetBytes(stringValue.Value);
                }
                else if (option.Value is CoapMessageOptionOpaqueValue opaqueValue)
                {
                    value = opaqueValue.Value;
                }
                else
                {
                    throw new CoAPProtocolViolationException("The specified option is not supported.");
                }

                var length = value.Length;

                EncodeOptionValue(delta, out var deltaNibble);
                writer.WriteBits(deltaNibble, 4);

                EncodeOptionValue(length, out var lengthNibble);
                writer.WriteBits(lengthNibble, 4);

                if (deltaNibble == 13)
                {
                    writer.WriteBits(delta - 13, 8);
                }
                else if (deltaNibble == 14)
                {
                    writer.WriteBits(delta - 269, 16);
                }

                if (lengthNibble == 13)
                {
                    writer.WriteBits(length - 13, 8);
                }
                else if (lengthNibble == 14)
                {
                    writer.WriteBits(length - 269, 16);
                }

                if (value.Length > 0)
                {
                    writer.WriteBytes(value);
                }
            }
        }

        byte[] EncodeUintOptioNValue(uint value)
        {
            if (value <= 255U)
            {
                return new byte[]
                {
                    (byte)value
                };
            }
            else if (value <= 65535U)
            {
                return new byte[]
                {
                    (byte)(value >> 8),
                    (byte)(value >> 0)
                };
            }
            else if (value <= 16777215U)
            {
                return new byte[]
                {
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)(value >> 0)
                };
            }
            else
            {
                return new byte[]
                {
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)(value >> 0)
                };
            }

            throw new CoAPProtocolViolationException("The value for the uint option is too long.");
        }

        void EncodeOptionValue(int value, out int nibble)
        {
            if (value <= 12)
            {
                nibble = value;
                return;
            }

            if (value <= 255 + 13)
            {
                nibble = 13;
                return;
            }

            if (value <= 65535 + 269)
            {
                nibble = 14;
                return;
            }

            throw new CoAPProtocolViolationException("Option value is too long.");
        }

        void ThrowIfInvalid(CoapMessage message)
        {
            if (message.Token?.Length > 8)
            {
                throw new CoAPProtocolViolationException("Message token is longer than 8 bytes.");
            }

            ThrowIfInvalid(message.Code);
        }

        void ThrowIfInvalid(CoapMessageCode code)
        {
            if (code.Class > 7)
            {
                throw new CoAPProtocolViolationException("Code class is larget than 7.");
            }

            if (code.Detail > 31)
            {
                throw new CoAPProtocolViolationException("Code class is larget than 7.");
            }
        }
    }
}