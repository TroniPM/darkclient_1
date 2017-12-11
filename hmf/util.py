#-*- coding:utf-8 -*-
'''
The MIT License (MIT)

Copyright(C) 2013 <Hooke HU>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'''
import struct
import array
import binascii

def hextoint(v):
		return int(binascii.b2a_hex(v), 16)

def ui8_to_int(v):
		return hextoint(v)
		
def ui32_to_int(v):
		v3 = hextoint(v[3])
		v2 = hextoint(v[2])
		v1 = hextoint(v[1])
		v0 = hextoint(v[0])
		r = (v3 << 24) | (v2 << 16) | (v1 << 8) | v0
		return int(r)
		
def int_to_ui8(v):
		a = array.array('B')
		a.append(v)
		return a.tostring()
		
def int_to_ui32(v):
		one = ((v << 24) >> 24) & 0x000000ff
		two = ((v << 16) >> 24) & 0x000000ff
		three = ((v << 8) >> 24) & 0x000000ff
		four = (v >> 24) & 0x00000000ff
		a = array.array('B')
		a.append(one)
		a.append(two)
		a.append(three)
		a.append(four)
		return a.tostring()
		
def write_varint32(v, stream):
		rst = []
		a = v & 0x7f
		if v >> 7 == 0:
				p = struct.pack('i', a & 0x7f)
				rst.append(p[0])
				stream.write(p[0])
				return rst
		else:
				p = struct.pack('i', a | 0x80)
				rst.append(p[0])
				stream.write(p[0])
		b = v << 18
		b = b >> 25
		if v >> 14 == 0:
				p = struct.pack('i', b & 0x7f)
				rst.append(p[0])
				stream.write(p[0])
				return rst
		else:
				p = struct.pack('i', b | 0x80)
				rst.append(p[0])
				stream.write(p[0])
		c = v << 11
		c = c >> 25
		if v >> 21 == 0:
				p = struct.pack('i', c & 0x7f)
				rst.append(p[0])
				stream.write(p[0])
				return rst
		else:
				p = struct.pack('i', c | 0x80)
				rst.append(p[0])
				stream.write(p[0])
		d = v << 4
		d = d >> 25
		if v >> 28 == 0:
				p = struct.pack('i', d & 0x7f)
				rst.append(p[0])
				stream.write(p[0])
				return rst
		else:
				p = struct.pack('i', d | 0x80)
				rst.append(p[0])
				stream.write(p[0])
		e = v >> 28 & 0x0f
		p = struct.pack('i', e)
		rst.append(p[0])
		stream.write(p[0])
		return rst
				
				
		
def read_varint32(stream):
		rst = 0
		v = stream.read(1)
		v = hextoint(v)
		b = v & 0x000000ff
		rst = b
		if rst & 0x00000080 != 0x00000080:
				return rst
		v = stream.read(1)
		v = hextoint(v)
		b = v & 0x000000ff
		rst = (rst & 0x0000007f) | (b << 7)
		if rst & 0x00004000 != 0x00004000:
				return rst
		v = stream.read(1)
		v = hextoint(v)
		b = v & 0x000000ff
		rst = (rst & 0x00003fff) | (b << 14)
		if rst & 0x00200000 != 0x00200000:
				return rst
		v = stream.read(1)
		v = hextoint(v)
		b = v & 0x000000ff
		rst = (rst & 0x001fffff) | (b << 21)
		if rst & 0x10000000 != 0x10000000:
				return rst
		v = stream.read(1)
		v = hextoint(v)
		b = v & 0x000000ff
		rst = (rst & 0x0fffffff) | (b << 28)
		return rst
		
		
def write_double(v, stream):
		p = struct.pack('d', v)
		stream.write(p)
		
def read_double(stream):
		v = stream.read(8)
		rst = struct.unpack('d', v)
		return rst[0]
		
def write_str(v, stream):
		stream.write(v)
		
def read_str(stream, length):
		return stream.read(length)
		
def zigzag32(v):
		"""sint to zigzag32"""
		return (v << 1)^(v >> 31)
		
def from_zigzag32(v):
		"""zigzag32 to sint"""
		return (v >> 1)^(v << 31)
		
def zigzag64(v):
		"""sint to zigzag64"""
		return (v << 1)^(v >> 63)
		
def from_zigzag64(v):
		"""zigzag64 to sint"""
		return (v >> 1)^(v << 63) 
		
if __name__ == "__main__":
		print 'u8_to_int'
		print ui8_to_int(struct.pack('B', 0x0f))
		print 'u32_to_int'
		print ui32_to_int(struct.pack('B', 0x0f) + struct.pack('B', 0x0e) + struct.pack('B', 0x01) + struct.pack('B', 0x02))
		print 'int_to_u8'
		v = int_to_ui8(15)
		print v
		print 'int_to_u32'
		v = int_to_ui32(33623567)
		print v
		print '------------'
		from StringIO import StringIO
		st = StringIO()
		rst = write_varint32(2147483648, st)
		print rst
		print st.len
		st.seek(0)
		print read_varint32(st)