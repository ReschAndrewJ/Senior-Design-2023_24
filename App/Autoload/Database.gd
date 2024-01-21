extends Node


const database_filename: String = "user://database.db"
var database = preload("res://addons/godot-sqlite/bin/gdsqlite.gdns").new()

const TABLE_WORDS_NAME: String = "WORDS_NAME"
const TABLE_WORDS_COLUMN_PRIMARY_KEY: String = "WORDS_COL_PKEY"
const TABLE_WORDS_COLUMN_KANJI: String = "WORDS_COL_KANJI"
const TABLE_WORDS_COLUMN_KANA: String = "WORDS_COL_KANA"

const TABLE_DEFINITIONS_NAME: String = "DEFINITIONS_NAME"
const TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY: String = "DEFINITIONS_COL_PKEY"
const TABLE_DEFINITIONS_COLUMN_WORD_KEY: String = "DEFINITIONS_COL_WORDKEY"
const TABLE_DEFINITIONS_COLUMN_TEXT: String = "DEFINITIONS_COL_TEXT"

const TABLE_GROUPS_NAME: String = "GROUPS_NAME"
const TABLE_GROUPS_COLUMN_PRIMARY_KEY: String = "GROUPS_COL_PKEY"
const TABLE_GROUPS_COLUMN_GROUP_NAME: String = "GROUPS_COL_GROUPNAME"

const TABLE_GROUPWORDPAIRS_NAME: String = "GWPAIRS_NAME"
const TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY: String = "GWPAIRS_COL_PKEY"
const TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY: String = "GWPAIRS_COL_WORDKEY"
const TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY: String = "GWPAIRS_COL_GROUPKEY"


func _ready():
	database.path = database_filename
	database.foreign_keys = true
	if not database.open_db():
		# database failed to open
		pass


	# Words Table
	if not database.query(
		"SELECT COUNT(name) FROM sqlite_master where type='table' and name='%s';" % TABLE_WORDS_NAME
		):
		# database failed to check if table exists
		pass
	if not database.query_result_by_reference[0].values()[0]:
		database.create_table(
			TABLE_WORDS_NAME,
			{
			TABLE_WORDS_COLUMN_PRIMARY_KEY : {"data_type":"int","primary_key":true, "auto_increment":true},
			TABLE_WORDS_COLUMN_KANJI : {"data_type":"text"},
			TABLE_WORDS_COLUMN_KANA : {"data_type":"text"}
			}
		)
	
	
	# Definitions Table
	if not database.query(
		"SELECT COUNT(name) FROM sqlite_master where type='table' and name='%s';" % TABLE_DEFINITIONS_NAME
		):
		# database failed to check if table exists
		pass
	if not database.query_result_by_reference[0].values()[0]:
		database.create_table(
			TABLE_DEFINITIONS_NAME,
			{ 
			TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY : {"data_type":"int","primary_key":true,"auto_increment":true},
			TABLE_DEFINITIONS_COLUMN_WORD_KEY : {"data_type":"int","foreign_key":"%s.%s" % [TABLE_WORDS_NAME, TABLE_WORDS_COLUMN_PRIMARY_KEY]},
			TABLE_DEFINITIONS_COLUMN_TEXT : {"data_type":"text"}
			}
		)

	
	# Groups Table
	if not database.query(
		"SELECT COUNT(name) FROM sqlite_master where type='table' and name='%s';" % TABLE_GROUPS_NAME
		):
		# database failed to check if table exists
		pass
	if not database.query_result_by_reference[0].values()[0]:
		database.create_table(
			TABLE_GROUPS_NAME,
			{
			TABLE_GROUPS_COLUMN_PRIMARY_KEY : {"data_type":"int","primary_key":true,"auto_increment":true},
			TABLE_GROUPS_COLUMN_GROUP_NAME : {"data_type":"text"}
			}
		)	


	# Group-Word Pairs Table
	if not database.query(
		"SELECT COUNT(name) FROM sqlite_master where type='table' and name='%s';" % TABLE_GROUPWORDPAIRS_NAME
		):
		# database failed to check if table exists
		pass
	if not database.query_result_by_reference[0].values()[0]:
		database.create_table(
			TABLE_GROUPWORDPAIRS_NAME,
			{
			TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY : {"data_type":"int","primary_key":true,"auto_increment":true},
			TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY : {"data_type":"int","foreign_key":"%s.%s" % [TABLE_WORDS_NAME, TABLE_WORDS_COLUMN_PRIMARY_KEY]},
			TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY : {"data_type":"int","foreign_key":"%s.%s" % [TABLE_GROUPS_NAME, TABLE_GROUPS_COLUMN_PRIMARY_KEY]}
			}
		)



func _notification(what):
	if (what == MainLoop.NOTIFICATION_WM_QUIT_REQUEST):
		if not database.close_db():
			# database failed to close
			pass
	elif (what == MainLoop.NOTIFICATION_APP_PAUSED):
		if not database.close_db():
			# database failed to close
			pass
	elif (what == MainLoop.NOTIFICATION_APP_RESUMED):
		if not database.open_db():
			# database failed to open
			pass


func insert_word(kanji: String, kana: String)->int:
	if not database.insert_row(
		TABLE_WORDS_NAME,
		{
		TABLE_WORDS_COLUMN_KANJI : kanji,
		TABLE_WORDS_COLUMN_KANA : kana
		}
	):
		# failed to insert word
		return -1
	return database.last_insert_rowid
	
	

#func update_word():
#func delete_word():


func insert_definition(word_key: int, text: String)->int:
	if not database.insert_row(
		TABLE_DEFINITIONS_NAME,
		{
		TABLE_DEFINITIONS_COLUMN_WORD_KEY : word_key,
		TABLE_DEFINITIONS_COLUMN_TEXT : text
		}
	):
		# failed to insert definition
		return -1
	return database.last_insert_rowid


#func update_definition():
#func delete_definition


func insert_group(gname: String)->int:
	if not database.insert_row(
		TABLE_GROUPS_NAME,
		{
		TABLE_GROUPS_COLUMN_GROUP_NAME : gname
		}
	):
		# failed to insert group
		return -1
	return database.last_insert_rowid


#func update_group
#func delete_group 


func get_groups(sortByName: bool = false)->Array:
	if not database.query(
		"SELECT %s, %s FROM %s ORDER BY %s;" % [
			TABLE_GROUPS_COLUMN_PRIMARY_KEY, TABLE_GROUPS_COLUMN_GROUP_NAME,
			TABLE_GROUPS_NAME, 
			(TABLE_GROUPS_COLUMN_GROUP_NAME if sortByName else TABLE_GROUPS_COLUMN_PRIMARY_KEY)
		]
	):
		# failed to query groups
		pass
	
	var groupCount = database.query_result.size()
	var pKeyArray = []
	pKeyArray.resize(groupCount)
	var gNameArray = []
	gNameArray.resize(groupCount)
	var result = [pKeyArray, gNameArray]
	var i: int = 0
	for gDict in database.query_result:
		var pKey = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_PRIMARY_KEY]
		var gName = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_GROUP_NAME]

		result[0][i] = pKey
		result[1][i] = gName
		i += 1
	return result 


func insert_groupWordPair(word_key: int, group_key: int):
	if not database.insert_row(
		TABLE_GROUPWORDPAIRS_NAME,
		{
		TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY : word_key,
		TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY : group_key
		}
	):
		# failed to insert pair
		return -1
	return database.last_insert_rowid


#func delete_groupWordPair


func get_wordcount():
	if not database.query("SELECT COUNT(%s) FROM %s" % [TABLE_WORDS_COLUMN_PRIMARY_KEY, TABLE_WORDS_NAME]):
		# failed to query word count
		pass
	return database.query_result_by_reference[0].values()[0]


func get_definitioncount():
	if not database.query("SELECT COUNT(%s) FROM %s" % [TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY, TABLE_DEFINITIONS_NAME]):
		# failed to query definition count
		pass
	return database.query_result_by_reference[0].values()[0]


func get_groupcount():
	if not database.query("SELECT COUNT(%s) FROM %s" % [TABLE_GROUPS_COLUMN_PRIMARY_KEY, TABLE_GROUPS_NAME]):
		# failed to query group count
		pass
	return database.query_result_by_reference[0].values()[0]


