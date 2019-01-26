import os
import yaml
import json
import random
import logging
import roslibpy
import websocket # sudo pip install websocket-client
from uuid import uuid4
from typing import List
from std_msgs.msg import String

class WebsocketROSPublisher(object):
	"""
	Class to send ROS messages to a ROS master that has a rosbridge.

	Author: Sammy Pfeiffer <Sammy.Pfeiffer at student.uts.edu.au>
	"""
	def __init__(self, websocket_ip, port=9090):
		"""
		Class to manage publishing to ROS thru a rosbridge websocket.
		:param str websocket_ip: IP of the machine with the rosbridge server.
		:param int port: Port of the websocket server, defaults to 9090.
		"""
		print("Connecting to websocket: {}:{}".format(websocket_ip, port))
		self.ws = websocket.create_connection(
			'ws://' + websocket_ip + ':' + str(port))
		self._advertise_dict = {}

	def _advertise(self, topic_name, topic_type):
		"""
		Advertise a topic with it's type in 'package/Message' format.
		:param str topic_name: ROS topic name.
		:param str topic_type: ROS topic type, e.g. std_msgs/String.
		:returns str: ID to de-advertise later on.
		"""
		new_uuid = str(uuid4())
		self._advertise_dict[new_uuid] = {'topic_name': topic_name,
										  'topic_type': topic_type}
		advertise_msg = {"op": "advertise",
						 "id": new_uuid,
						 "topic": topic_name,
						 "type": topic_type
						 }
		self.ws.send(json.dumps(advertise_msg))
		return new_uuid

	def _unadvertise(self, uuid):
		unad_msg = {"op": "unadvertise",
					"id": uuid,
					# "topic": topic_name
					}
		self.ws.send(json.dumps(unad_msg))

	def __del__(self):
		"""Cleanup all advertisings"""
		d = self._advertise_dict
		# for k in d:
		#     self._unadvertise(k)

	def _publish(self, topic_name, message):
		"""
		Publish onto the already advertised topic the msg in the shape of
		a Python dict.
		:param str topic_name: ROS topic name.
		:param dict msg: Dictionary containing the definition of the message.
		"""
		msg = {
			'op': 'publish',
			'topic': topic_name,
			'msg': message
		}
		json_msg = json.dumps(msg)
		self.ws.send(json_msg)

	def publish(self, topic_name, ros_message):
		"""
		Publish on a topic given ROS message thru rosbridge.
		:param str topic_name: ROS topic name.
		:param * ros_message: Any ROS message instance, e.g. LaserScan()
			from sensor_msgs/LaserScan.
		"""
		# First check if we already advertised the topic
		d = self._advertise_dict
		for k in d:
			if d[k]['topic_name'] == topic_name:
				# Already advertised, do nothing
				break
		else:
			# Not advertised, so we advertise
			topic_type = ros_message._type
			self._advertise(topic_name, topic_type)
		# Converting ROS message to a dictionary thru YAML
		ros_message_as_dict = yaml.load(ros_message.__str__())
		# Publishing
		self._publish(topic_name, ros_message_as_dict)


class Vector3:
	def __init__(self, x: float = 0., y: float = 0., z: float = 0.):
		self.x = x
		self.y = y
		self.z = z

	def __add__(self, other: 'Vector3'):
		return Vector3(self.x + other.x, self.y + other.y, self.z + other.z)

	def __iadd__(self, other: 'Vector3'):
		self = self + other
		return self

	def __sub__(self, other: 'Vector3'):
		return Vector3(self.x - other.x, self.y - other.y, self.z - other.z)

	def __neg__(self):
		return Vector3(-self.x, -self.y, -self.z)

	def __mul__(self, other: float):
		return Vector3(self.x * other, self.y * other, self.z * other)

	def __rmul__(self, other: float):
		return Vector3(self.x * other, self.y * other, self.z * other)

	def __imul__(self, other: float):
		self = self * other
		return self

	def __turediv__(self, other: float):
		return Vector3(self.x / other, self.y / other, self.z / other)

	def __itruedif__(self, other: float):
		self = self / other
		return self


class BasicObject:
	def __init__(self, id_: int, type_: int, angle: float, probability: float, position: 'Vector3' = Vector3()):
		self.id_ = id_
		self.type_ = type_
		self.position = position
		self.angle = angle
		self.probability = probability

	def to_json_string(self):
		data = {"id": self.id_, "type": self.type_, "x": self.position.x, "y": self.position.y, "z": self.position.z,
				"th": self.angle, "p": self.probability}
		return data


class DataFrame:
	def __init__(self, timestamp: int, objects: List[BasicObject], depth: float, rotation: float,
				 position: 'Vector3' = Vector3(), acceleration: 'Vector3' = Vector3(), speed: 'Vector3' = Vector3()):
		self.timestamp = timestamp
		self.objects = objects
		self.position = position
		self.acceleration = acceleration
		self.speed = speed
		self.depth = depth
		self.rotation = rotation

	def to_json(self):
		# objects_json = []
		# for obj in self.objects:
		#     objects_json.append(obj.to_json())

		data = {"ts": self.timestamp, "objects": self.objects, "x": self.position.x, "y": self.position.y,
				"z": self.position.z, "ax": self.acceleration.x, "ay": self.acceleration.y, "az": self.acceleration.z,
				"ux": self.speed.x, "uy": self.speed.y, "uz": self.speed.z, "d": self.depth, "th": self.rotation}
		return json.dumps(data, default=lambda x: x.to_json_string(), sort_keys=False, indent=4)


# Current algorithm generates n objects and describes them as the Turtle moves in a randomly
# generated path. Objects are created at random static positions and locations change in relation
# to the Turtle, as it moves.
# Angle and probability are not accounted for at this time.
def random_world_gen(frame_count: int, object_count: int = 10, type_count: int = 3):
	timestamp = 0
	object_list = []
	current_position = Vector3(0, 0, 0)
	current_acceleration = Vector3(0, 0, 0)  # Unaccounted
	current_speed = Vector3(0, 0, 0)  # Unaccounted
	current_depth = 0
	current_rotation = 0  # Unaccounted

	# Create n objects
	for i in range(object_count):
		obj = BasicObject(i, random.randrange(0, type_count), 0, 1)
		obj.position = Vector3(random.uniform(-10., 10.), random.uniform(0., 10.), random.uniform(-10., 10.))

		# Constant angle and probability for now
		obj.angle = 0
		obj.probability = 1
		object_list.append(obj)

	# Simulate n time frames
	for i in range(frame_count):
		delta_t = random.randrange(1000, 2000)
		delta_pos = Vector3(random.random(), random.random(), random.random()) * (delta_t / 1000.)
		timestamp += delta_t

		# Turtle moves by delta_pos and everything shifts around it
		current_position += delta_pos

		for obj in object_list:
			obj.position -= delta_pos

		# Write frame object to file
		frame_obj = DataFrame(timestamp, object_list, current_depth, current_rotation, current_position,
							  current_acceleration, current_speed)

		frame_json = frame_obj.to_json()
		file_name = "{:0>6}.json".format(i)
		frame_file = open(file_name, 'w')
		frame_file.write(frame_json)
		frame_file.close()

def ros_random_message_gen(object_count: int = 10, type_count: int = 3):
	pub = WebsocketROSPublisher('localhost')

	for i in range(2):
		timestamp = 0
		object_list = []
		current_position = Vector3(0, 0, 0)
		current_acceleration = Vector3(0, 0, 0)  # Unaccounted
		current_speed = Vector3(0, 0, 0)  # Unaccounted
		current_depth = 0
		current_rotation = 0  # Unaccounted

		# Create n objects
		for i in range(object_count):
			obj = BasicObject(i, random.randrange(0, type_count), 0, 1)
			obj.position = Vector3(random.uniform(-1., 1.), random.uniform(0., 1.), random.uniform(-1., 1.))

			# Constant angle and probability for now
			obj.angle = 0
			obj.probability = 1
			object_list.append(obj)

		# Simulate n time frame
		delta_t = random.randrange(1000, 2000)
		delta_pos = Vector3(random.random(), random.random(), random.random()) * (delta_t / 1000.)
		timestamp += delta_t

		# Turtle moves by delta_pos and everything shifts around it
		current_position += delta_pos

		for obj in object_list:
			obj.position -= delta_pos

		# Write frame object to file
		frame_obj = DataFrame(timestamp, object_list, current_depth, current_rotation, current_position, current_acceleration, current_speed)
		
		#rospy.loginfo(frame_obj.to_json())
		frame_str = String(frame_obj.to_json())
		pub.publish('/listener', frame_str)


def auto_generate():
	frame_count = int(input('Enter frame count:'))
	random_world_gen(frame_count)
	print("Frames generated!\n")
	input("Press Enter to continue...")


def callback(data):
	print(data.data)


def menu():
	while True:
		print("==============================")
		print("     Mock Stream Generator    ")
		print("==============================")
		print("1. Auto generate stream")
		print("2. Live generate stream")
		print("3. Exit")
		print("==============================")
		selection = int(input('Enter your input:'))
		print(selection)
		if selection == 1:
			auto_generate()
		elif selection == 2:
			ros_random_message_gen()
		elif selection == 3:
			break

		os.system('cls')  # For windows systems
		# os.system('clear')  # For unix based system


if __name__ == "__main__":
	logging.getLogger().setLevel(logging.INFO)
	menu()