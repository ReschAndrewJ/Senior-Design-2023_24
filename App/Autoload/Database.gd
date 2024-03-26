extends Node


const sample_database: String = "res://_sample_database/database.db"
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
	var dir = Directory.new()
	if not dir.file_exists(database_filename):
		if dir.file_exists(sample_database):
			dir.copy(sample_database, database_filename)


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
	
	

func update_word(word_pKey: int, kanji: String, kana: String)->bool:
	var dict: Dictionary = { 
		TABLE_WORDS_COLUMN_KANJI: kanji, 
		TABLE_WORDS_COLUMN_KANA: kana
	};
	if not database.update_rows(
		TABLE_WORDS_NAME, "%s = %s" % [TABLE_WORDS_COLUMN_PRIMARY_KEY, word_pKey],
		dict
	):
		# failed to update group
		return false
	return true


func delete_word(word_pKey: int)->bool:
	if not database.delete_rows(
		TABLE_WORDS_NAME, "%s = %s" % [TABLE_WORDS_COLUMN_PRIMARY_KEY, word_pKey]
	):
		# failed to delete word
		return false
	return true



func get_words(pKeys: Array)->Array:
	var pKeyArray: Array = []
	var kanjiArray: Array = []
	var kanaArray: Array = []

	if pKeys.size() == 0:
		var result: Array = [pKeyArray, kanjiArray, kanaArray]
		return result

	var pKeysString: String = "(";
	pKeysString = pKeysString + "%s" % pKeys[0]
	for i in pKeys.size()-1:
		pKeysString = pKeysString + ", %s" % pKeys[i+1]
	pKeysString = pKeysString + ")"

	if not database.query(
		"SELECT %s, %s, %s FROM %s WHERE %s IN %s;" % [
			TABLE_WORDS_COLUMN_PRIMARY_KEY, TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANA,
			TABLE_WORDS_NAME, TABLE_WORDS_COLUMN_PRIMARY_KEY, pKeysString
		]
	): 
		# failed to query words
		pass
	
	var wordCount: int = database.query_result.size()
	pKeyArray.resize(wordCount)
	kanjiArray.resize(wordCount)
	kanaArray.resize(wordCount)

	var i: int = 0
	for wDict in database.query_result:
		var pKey = (wDict as Dictionary)[TABLE_WORDS_COLUMN_PRIMARY_KEY]
		var kanji = (wDict as Dictionary)[TABLE_WORDS_COLUMN_KANJI]
		var kana = (wDict as Dictionary)[TABLE_WORDS_COLUMN_KANA]

		pKeyArray[i] = pKey
		kanjiArray[i] = kanji
		kanaArray[i] = kana
		
		i += 1

	var result: Array = [pKeyArray, kanjiArray, kanaArray]
	return result



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


func get_wordDefinitions(word_key: int)->Array:
	var pKeyArray: Array = []
	var textArray: Array = []

	if not database.query(
		"SELECT %s, %s FROM %s WHERE %s=%s;" % [
			TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY,  TABLE_DEFINITIONS_COLUMN_TEXT,
			TABLE_DEFINITIONS_NAME, TABLE_DEFINITIONS_COLUMN_WORD_KEY, word_key
		]
	):
		# failed to query definitions
		pass
	
	var wordCount: int = database.query_result.size()
	pKeyArray.resize(wordCount)
	textArray.resize(wordCount)

	var i: int = 0
	for dDict in database.query_result:
		var definition_pKey = (dDict as Dictionary)[TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY]
		var text = (dDict as Dictionary)[TABLE_DEFINITIONS_COLUMN_TEXT]

		pKeyArray[i] = definition_pKey
		textArray[i] = text

		i += 1
	
	var result: Array = [pKeyArray, textArray]
	return result


#func update_definition():
func delete_definitions(pKeys: Array)->bool:
	if pKeys.size() == 0:
		return false

	var pKeysString: String = "(";
	pKeysString = pKeysString + "%s" % pKeys[0]
	for i in pKeys.size()-1:
		pKeysString = pKeysString + ", %s" % pKeys[i+1]
	pKeysString = pKeysString + ")"

	if not database.delete_rows(
		TABLE_DEFINITIONS_NAME, "%s IN %s" % [TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY, pKeysString]
	):
		# failed to delete definitions
		return false
	return true



func insert_group(group_name: String)->int:
	if not database.insert_row(
		TABLE_GROUPS_NAME,
		{
		TABLE_GROUPS_COLUMN_GROUP_NAME : group_name
		}
	):
		# failed to insert group
		return -1
	return database.last_insert_rowid


func update_group(group_pKey: int, group_name: String)->bool:
	var dict: Dictionary = { TABLE_GROUPS_COLUMN_GROUP_NAME: group_name };
	if not database.update_rows(
		TABLE_GROUPS_NAME, "%s = %s" % [TABLE_GROUPS_COLUMN_PRIMARY_KEY, group_pKey],
		dict
	):
		# failed to update group
		return false
	return true


func delete_group(group_pKey: int)->bool:
	if not database.delete_rows(
		TABLE_GROUPS_NAME, "%s = %s" % [TABLE_GROUPS_COLUMN_PRIMARY_KEY, group_pKey]
	):
		# failed to delete group
		return false
	return true


func get_groups()->Array:
	if not database.query(
		"SELECT %s, %s FROM %s;" % [
			TABLE_GROUPS_COLUMN_PRIMARY_KEY, TABLE_GROUPS_COLUMN_GROUP_NAME,
			TABLE_GROUPS_NAME
		]
	):
		# failed to query groups
		pass
	
	var groupCount = database.query_result.size()
	var pKeyArray = []
	var gNameArray = []
	pKeyArray.resize(groupCount)
	gNameArray.resize(groupCount)
	
	var i: int = 0
	for gDict in database.query_result:
		var pKey = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_PRIMARY_KEY]
		var gName = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_GROUP_NAME]

		pKeyArray[i] = pKey
		gNameArray[i] = gName
		i += 1
	
	var result: Array = [pKeyArray, gNameArray]
	return result


func get_groupsByKeys(group_keys: Array)->Array:
	var pKeyArray = []
	var gNameArray = []

	if group_keys.size() == 0:
		var result: Array = [pKeyArray, gNameArray]
		return result

	var pKeysString: String = "(";
	pKeysString = pKeysString + "%s" % group_keys[0]
	for i in group_keys.size()-1:
		pKeysString = pKeysString + ", %s" % group_keys[i+1]
	pKeysString = pKeysString + ")"

	if not database.query(
		"SELECT %s, %s FROM %s WHERE %s IN %s;" % [
			TABLE_GROUPS_COLUMN_PRIMARY_KEY, TABLE_GROUPS_COLUMN_GROUP_NAME,
			TABLE_GROUPS_NAME, TABLE_GROUPS_COLUMN_PRIMARY_KEY, pKeysString
		]
	):
		# failed to query groups
		pass
	
	var groupCount = database.query_result.size()
	pKeyArray.resize(groupCount)
	gNameArray.resize(groupCount)

	var i: int = 0
	for gDict in database.query_result:
		var pKey = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_PRIMARY_KEY]
		var gName = (gDict as Dictionary)[TABLE_GROUPS_COLUMN_GROUP_NAME]

		pKeyArray[i] = pKey
		gNameArray[i] = gName
		i += 1

	var result: Array = [pKeyArray, gNameArray]
	return result



func get_wordKeysInGroup(group_key: int, limit=0)->Array:
	if not database.query(
		"SELECT %s FROM %s WHERE %s=%s %s;" % [
			TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY, TABLE_GROUPWORDPAIRS_NAME,
			TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY, group_key,
			
			("ORDER BY %s LIMIT %s" % [TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY, limit]) 
				if limit != 0 else ""
		]
	):
		# failed to query group-word pairs
		pass
	
	var pairCount = database.query_result.size()
	var wordKeyArray: Array = []
	wordKeyArray.resize(pairCount)
	var i: int = 0
	for pairDict in database.query_result:
		var pKey = (pairDict as Dictionary)[TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY]
		wordKeyArray[i] = pKey
		i += 1
	return wordKeyArray

func get_groupKeysWithWord(word_key: int)->Array:
	if not database.query(
		"SELECT %s FROM %s WHERE %s=%s;" % [
			TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY, TABLE_GROUPWORDPAIRS_NAME,
			TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY, word_key
		]
	):
		# failed to query group-word pairs
		pass
	
	var pairCount = database.query_result.size()
	var groupKeyArray: Array = []
	groupKeyArray.resize(pairCount)
	var i: int = 0
	for pairDict in database.query_result:
		var pKey = (pairDict as Dictionary)[TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY]
		groupKeyArray[i] = pKey
		i += 1
	return groupKeyArray 




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


func delete_groupWordPairs(pKeys: Array)->bool:
	if pKeys.size() == 0:
		return false

	var pKeysString: String = "(";
	pKeysString = pKeysString + "%s" % pKeys[0]
	for i in pKeys.size()-1:
		pKeysString = pKeysString + ", %s" % pKeys[i+1]
	pKeysString = pKeysString + ")"

	if not database.delete_rows(
		TABLE_GROUPWORDPAIRS_NAME, "%s IN %s" % [TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY, pKeysString]
	):
		# failed to delete pairs
		return false
	return true

func get_groupWordPairsForGroup(group_pKey: int)->Array:
	if not database.query(
		"SELECT %s FROM %s WHERE %s=%s;" % [
			TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY, TABLE_GROUPWORDPAIRS_NAME,
			TABLE_GROUPWORDPAIRS_COLUMN_GROUP_KEY, group_pKey 
		]
	):
		# failed to query groupWordPairs
		var res: Array = []
		return res
	
	var pairCount = database.query_result.size()
	var res: Array = []
	res.resize(pairCount)

	var i: int = 0
	for pairDict in database.query_result:
		var pKey = (pairDict as Dictionary)[TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY]
		res[i] = pKey
		i += 1
	
	return res

func get_groupWordPairsForWord(word_pKey: int)->Array:
	if not database.query(
		"SELECT %s FROM %s WHERE %s=%s;" % [
			TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY, TABLE_GROUPWORDPAIRS_NAME,
			TABLE_GROUPWORDPAIRS_COLUMN_WORD_KEY, word_pKey 
		]
	):
		# failed to query groupWordPairs
		var res: Array = []
		return res
	
	var pairCount = database.query_result.size()
	var res: Array = []
	res.resize(pairCount)

	var i: int = 0
	for pairDict in database.query_result:
		var pKey = (pairDict as Dictionary)[TABLE_GROUPWORDPAIRS_COLUMN_PRIMARY_KEY]
		res[i] = pKey
		i += 1
	
	return res


func get_wordcount():
	if not database.query("SELECT COUNT(%s) FROM %s;" % [TABLE_WORDS_COLUMN_PRIMARY_KEY, TABLE_WORDS_NAME]):
		# failed to query word count
		pass
	return database.query_result_by_reference[0].values()[0]


func get_definitioncount():
	if not database.query("SELECT COUNT(%s) FROM %s;" % [TABLE_DEFINITIONS_COLUMN_PRIMARY_KEY, TABLE_DEFINITIONS_NAME]):
		# failed to query definition count
		pass
	return database.query_result_by_reference[0].values()[0]


func get_groupcount():
	if not database.query("SELECT COUNT(%s) FROM %s;" % [TABLE_GROUPS_COLUMN_PRIMARY_KEY, TABLE_GROUPS_NAME]):
		# failed to query group count
		pass
	return database.query_result_by_reference[0].values()[0]






func get_wordsByKanaKanjiSearch(search_text: String):
	var pKeyArray: Array = []
	var kanjiArray: Array = []
	var kanaArray: Array = []
	
	if search_text.length() == 0:
		var resArray = [pKeyArray, kanjiArray, kanaArray]
		return resArray
	

	var query_string_select: String = "SELECT %s, %s, %s FROM %s " % [
		TABLE_WORDS_COLUMN_PRIMARY_KEY, TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANA,
		TABLE_WORDS_NAME 
	]
	var query_string_filterA: String = "WHERE (%s LIKE ? || '%%') OR (%s LIKE ? || '%%') OR " % [
		TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANA
	]
	var query_string_filterB: String = "(length(%s)>1 AND ? LIKE substr(%s, 1, length(%s)-1) || '%%') OR (length(%s)>1 AND ? LIKE substr(%s, 1, length(%s)-1) || '%%') " % [
		TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANJI, 
		TABLE_WORDS_COLUMN_KANA, TABLE_WORDS_COLUMN_KANA, TABLE_WORDS_COLUMN_KANA
	]
	var query_string_order: String = "ORDER BY %s;" % [
		TABLE_WORDS_COLUMN_PRIMARY_KEY
	]

	var query_string: String = query_string_select + query_string_filterA + query_string_filterB + query_string_order
	var param_bindings: Array = [search_text, search_text, search_text, search_text]

	if not database.query_with_bindings(query_string, param_bindings):
		# failed to query search
		pass
	
	var wordCount: int = database.query_result.size()
	pKeyArray.resize(wordCount)
	kanjiArray.resize(wordCount)
	kanaArray.resize(wordCount)

	var i: int = 0
	for wDict in database.query_result:
		var pKey = (wDict as Dictionary)[TABLE_WORDS_COLUMN_PRIMARY_KEY]
		var kanji = (wDict as Dictionary)[TABLE_WORDS_COLUMN_KANJI]
		var kana = (wDict as Dictionary)[TABLE_WORDS_COLUMN_KANA]

		pKeyArray[i] = pKey
		kanjiArray[i] = kanji
		kanaArray[i] = kana
		
		i += 1

	var result: Array = [pKeyArray, kanjiArray, kanaArray]
	return result


func get_wordsByDefinitionSearch(search_text: String):
	var wordPKeyArray: Array = []
	var kanjiArray: Array = []
	var kanaArray: Array = []
	var definitionTextArray: Array = []

	if search_text.length() == 0:
		var resArray = [wordPKeyArray, kanjiArray, kanaArray, definitionTextArray]
		return resArray

	var query_string: String = "SELECT %s, %s, %s, %s FROM %s LEFT JOIN %s ON %s.%s=%s.%s WHERE %s LIKE '%%' || ? || '%%' ORDER BY %s;" % [
		TABLE_DEFINITIONS_COLUMN_WORD_KEY, TABLE_DEFINITIONS_COLUMN_TEXT, TABLE_WORDS_COLUMN_KANJI, TABLE_WORDS_COLUMN_KANA,
		TABLE_DEFINITIONS_NAME, TABLE_WORDS_NAME,
		TABLE_DEFINITIONS_NAME, TABLE_DEFINITIONS_COLUMN_WORD_KEY, TABLE_WORDS_NAME, TABLE_WORDS_COLUMN_PRIMARY_KEY,
		TABLE_DEFINITIONS_COLUMN_TEXT,
		TABLE_DEFINITIONS_COLUMN_WORD_KEY
	]
	var param_bindings: Array = [search_text]

	if not database.query_with_bindings(query_string, param_bindings):
		# failed to query search
		pass
	
	var defCount: int = database.query_result.size()
	wordPKeyArray.resize(defCount)
	kanjiArray.resize(defCount)
	kanaArray.resize(defCount)
	definitionTextArray.resize(defCount)

	var i: int = 0
	for dDict in database.query_result:
		var wpKey = (dDict as Dictionary)[TABLE_DEFINITIONS_COLUMN_WORD_KEY]
		var kanji = (dDict as Dictionary)[TABLE_WORDS_COLUMN_KANJI]
		var kana = (dDict as Dictionary)[TABLE_WORDS_COLUMN_KANA]
		var text = (dDict as Dictionary)[TABLE_DEFINITIONS_COLUMN_TEXT]
		
		wordPKeyArray[i] = wpKey
		kanjiArray[i] = kanji
		kanaArray[i] = kana
		definitionTextArray[i] = text
		
		i += 1

	var result = [wordPKeyArray, kanjiArray, kanaArray, definitionTextArray]
	return result
