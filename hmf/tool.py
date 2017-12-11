#-*- coding:utf-8 -*-
'''
The MIT License (MIT)

Copyright(C) 2013 <Hooke HU>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'''
from xml.etree import ElementTree as etree
from hmf import Hmf

subfixs = ['_i', '_f', '_s', '_l', '_k', '_m']

def read_cfg(filename):
		fd = open(filename, "rb")
		content = fd.read()
		fd.close()
		x = etree.XML(content)
		rst = {}
		roots = x.getchildren()
		for r in roots:
				children = r.getchildren()
				if len(children) == 0:
						continue
				r = {}
				for i in children:
						if i.tag[-2:] in subfixs:
								r[i.tag[:-2]] = convert_value(i.tag, i.text)
						else:
								r[i.tag] = convert_value(i.tag, i.text)
				if not r.has_key('id_i') and not r.has_key('id'):
						continue
				if r.has_key('id_i'):
						rst[r['id_i']] = r
				else:
						rst[r['id']] = r
		return rst
		
def convert_value(key, value):
		subfix = key[-2:]
		if value is None:
				return ""
		if isinstance(value, unicode):
				return value.encode('utf-8')
		return value
		#under line not use
		if subfix == "_i":
				if value == "" or value is None:
						return 0
				try:
						return int(value)
				except:
						return float(value)
		elif subfix == "_f":
				if value == "" or value is None:
						return 0.0
				return float(value)
		elif subfix == "_l":
				if value is None:
						return []
				if not isinstance(value, str) and not isinstance(value, unicode):
						return [int(value)]
				l = value.split(",")
				for i in range(len(l)):
						l[i] = int(l[i])
				return l
		elif subfix == "_k":
				if value is None:
						return []
				if not isinstance(value, str) and not isinstance(value, unicode):
						return [int(value)]
				k = value.split(",")
				for i in range(len(k)):
						k[i] = int(k[i])
				return k
		elif subfix == "_m":
				if value == "" or value is None:
						return {}
				m = value.split(",")
				rv = {}
				for i in range(len(m)):
						kv = m[i].split(":")
						if kv[0].isdigit():
								k1 = int(kv[0])
						else:
								k1 = kv[0]
						if kv[1].isdigit():
								v1 = int(kv[1])
						else:
								v1 = kv[1]
						rv[k1] = v1
				return rv
		elif subfix == "_s":
				if value is None:
						return ''
				return value
		else:
				if value is None:
						return ''
				return value
				
def read_def(filename):
		fd = open(filename, "rb")
		content = fd.read()
		fd.close()
		x = etree.XML(content)
		rst = {}
		roots = x.getchildren()
		'''
				{
				Properties:[{name:[[Type, STRING], [Flags, BASE_AND_CLIENT]]}, {platAccount:[[Type, STRING], [Flags, BASE_AND_CLIENT]]}],
				ClientMethods:[{OnLogoutResp:[[Arg, UINT8], [Arg, UINT32]], RandomNameResp:[[Arg, STRING]]}],
				BaseMethods:[],
				CellMathods:[]
				}
		'''
		for r in roots:
				fathers = r.getchildren()
				if len(fathers) == 0:
						continue
				father_list = []
				for father in fathers:
						suns = father.getchildren()
						father_dict = {}
						sun_list = []
						for sun in suns:
								tv = ''
								if sun.text is not None:
										tv = sun.text
								sun_list.append([sun.tag, tv])
						father_dict[father.tag] = sun_list
						father_list.append(father_dict)
				rst[r.tag] = father_list
		return rst
		
def read_entities(filename):
		fd = open(filename, "rb")
		content = fd.read()
		fd.close()
		x = etree.XML(content)
		rst = {}
		roots = x.getchildren()
		l = []
		for r in roots:
				l.append(r.tag)
		rst['entities'] = l
		return rst
		
if __name__ == "__main__":
		import sys, os, dircache
		from StringIO import StringIO
		print sys.argv
		base_path = '''E:/mogo/doc/product/配置表/xml文件最终版/'''
		out_path = '''E:/hmfoutput/'''
		def_path = '''E:/mogo/src/trunk/client/Assets/Resources/data/entity_defs/'''
		def_out_path = '''E:/defoutput/'''
		if len(sys.argv) > 2:
				base_path = sys.argv[1]
				out_path = sys.argv[2]
				def_path = sys.argv[3]
				def_out_path = sys.argv[4]
		files = dircache.listdir(unicode(base_path, "utf-8"))
		for name in files:
				if name == '.svn':
						continue
				if name[-4:] != '.xml':
						continue
				print name
				r = read_cfg(unicode(base_path, "utf-8") + name)
				stream = StringIO()
				#print r
				h = Hmf()
				h.write_object(r, stream)
				stream.seek(0)
				fd = open(out_path + name[:-4] + '.xml', "wb")
				fd.write(stream.read())
				fd.close()
		files = dircache.listdir(unicode(def_path, "utf-8"))
		for name in files:
				if name == '.svn':
						continue
				if name[-4:] != ".xml":
						continue
				if name == "entities.xml":
						r = read_entities(unicode(def_path, "utf-8") + name)
				else:
						r = read_def(unicode(def_path, "utf-8") + name)
				stream = StringIO()
				h = Hmf()
				h.write_object(r, stream)
				stream.seek(0)
				fd = open(def_out_path + name[:-4] + '.xml', "wb")
				fd.write(stream.read())
				fd.close()