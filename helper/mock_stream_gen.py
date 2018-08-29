from typing import List
import random

class JSONable:

	def __repr__(self):
		return str(self.__dict__)

	def __iter__(self):
		for attr in dir(self):
			if not attr.startswith('__') and not callable(getattr(self, attr)):
				value = getattr(self, attr)
				if not isinstance(value, (dict, list, str, int, float, bool)) and value != None:
					value = dict(value)
				yield attr, value

	def __repr__(self):
		return str(self.__dict__)

	def toJSON(self, indent: int = -1):
		json_str = str(dict(self)).replace('\'', '\"')
		json_str = self.__prettify__(json_str, indent)
		return json_str

	def __prettify__(self, ugly, indent):
		# -1 for \t as indent
		inds = 0
		ind_str = ''
		pretty = ''
		if indent == 0:
			return ugly
		if indent < 0:
			ind_str = '\t'
		else:
			ind_str = ' ' * indent
		for c in ugly:
			if c == '{':
				inds += 1
				pretty += '{\n' + inds * ind_str
			elif c == '[':
				inds += 1
				pretty += '[\n' + inds * ind_str
			elif c == ' ':
				pass
			elif c == ':':
				pretty += ': '
			elif c == ',':
				pretty += ',\n' + inds * ind_str
			elif c == '}':
				inds -= 1
				pretty += '\n' + inds * ind_str + '}'
			elif c == ']':
				inds -= 1
				pretty += '\n' + inds * ind_str + ']'
			else:
				pretty += c
		return pretty

class vector3(JSONable):

	def __init__(self, x: float = 0., y: float = 0., z: float = 0.):
		self.x = x
		self.y = y
		self.z = z

	def __add__(self, other: 'vector3'):
		return vector3(self.x + other.x, self.y + other.y, self.z + other.z)

	def __iadd__(self, other: 'vector3'):
		self = self + other
		return self

	def __sub__(self, other: 'vector3'):
		return vector3(self.x - other.x, self.y - other.y, self.z - other.z)

	def __neg__(self):
		return vector3(-self.x, -self.y, -self.z)

	def __mul__(self, other: float):
		return vector3(self.x * other, self.y * other, self.z * other)

	def __rmul__(self, other: float):
		return vector3(self.x * other, self.y * other, self.z * other)

	def __imul__(self, other: float):
		self = self * other
		return self

	def __turediv__(self, other: float):
		return vector3(self.x / other, self.y / other, self.z / other)

	def __itruedif__(self, other: float):
		self = self / other
		return self

	
class basic_object(JSONable):

	def __init__(self, id: int, type: int, \
		position: 'vector3' = vector3(), orientation: 'vector3' = vector3(), \
		linear_velocity: 'vector3' = vector3(), angular_velocity: 'vector3' = vector3(), \
		linear_acceleration: 'vector3' = vector3(), angular_acceleration: 'vector3' = vector3()):
		self.id = id
		self.type = type
		self.position = position
		self.orientation = orientation
		self.linear_velocity = linear_velocity
		self.angular_velocity = angular_velocity
		self.linear_acceleration = linear_acceleration
		self.angular_acceleration = angular_acceleration


class world_status(JSONable):

	def __init__(self, timestamp: int, objects: List[basic_object]):
		self.timestamp = timestamp
		self.objects = objects


def random_world_gen(frame_count: int, object_count : int = 10, type_count: int = 3):
	timestamp = 0
	object_list = []
	for i in range(object_count):
		obj = basic_object(i, random.randrange(0, type_count))
		obj.position = vector3(random.uniform(-10., 10.), random.uniform(0., 10.), random.uniform(-10., 10.))
		obj.orientation = vector3(random.uniform(0., 360.), random.uniform(0., 360.), random.uniform(0., 360.))
		object_list.append(obj)

	for i in range(frame_count):
		delta_t = random.randrange(1000, 2000)
		timestamp += delta_t
		# object_count_for_frame = random.randrange(0, 10)
		# object_list_for_frame = random.sample(object_list, object_count_for_frame)
		# for obj in object_list_for_frame:
		for obj in object_list:
			delta_pos = vector3(random.random(), random.random(), random.random()) * (delta_t / 1000.)
			delta_ori = vector3(random.random(), random.random(), random.random()) * (delta_t / 1000.)
			obj.position += delta_pos
			obj.orientation += delta_ori

		# frame_obj = world_status(timestamp, object_list_for_frame)
		frame_obj = world_status(timestamp, object_list)
		frame_json = frame_obj.toJSON()
		file_name = "{:0>6}.json".format(i)
		frame_file = open(file_name, 'w')
		frame_file.write(frame_json)
		frame_file.close()


if __name__ == "__main__":
	random_world_gen(100)
