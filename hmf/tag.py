#-*- conding:utf-8 -*-
'''
The MIT License (MIT)

Copyright(C) 2013 <Hooke HU>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'''
INT_TAG = 0x00 #0
DOUBLE_TAG = 0x01 #1
NULL_TAG = 0x02 #2
BYTES_TAG = 0x03 #3
BOOL_TAG = 0x04 #4
OBJECT_TAG = 0x05 #5
ARRAY_TAG = 0x06 #6
STRING_TAG = 0x07 #7

def show_tag(tag_type):
		if tag_type == INT_TAG:
				print "INT_TAG"
		elif tag_type == DOUBLE_TAG:
				print "DOUBLE_TAG"
		elif tag_type == NULL_TAG:
				print "NULL_TAG"
		elif tag_type == BYTES_TAG:
				print "BYTES_TAG"
		elif tag_type == BOOL_TAG:
				print "BOOL_TAG"
		elif tag_type == OBJECT_TAG:
				print "OBJECT_TAG"
		elif tag_type == ARRAY_TAG:
				print "ARRAY_TAG"
		elif tag_type == STRING_TAG:
				print "STRING_TAG"
		else:
				print "UNKNOWN TAG"