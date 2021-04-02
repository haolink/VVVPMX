#Black Clover Quartet Knights
#original by chrrox
#modified for VVVTune by haolink
from inc_noesis import *
import collections

def registerNoesisTypes():
	handle = noesis.register("Orochi 4 MDL", ".mdl")
	noesis.setHandlerTypeCheck(handle, gsmdlCheckType)
	noesis.setHandlerLoadModel(handle, gsmdlLoadModel)

	handle = noesis.register("Orochi 4 TEX", ".tex")
	noesis.setHandlerTypeCheck(handle, gstexCheckType)
	noesis.setHandlerLoadRGBA(handle, gstexLoadRGBA)
	noesis.logPopup()

	return 1


def gsmdlCheckType(data):
	td = NoeBitStream(data)
	return 1

def gstexCheckType(data):
	td = NoeBitStream(data)
	return 1

class gsmdlFile: 
         
	def __init__(self, bs):
		self.bs = bs
		self.texList  = []
		self.matList  = []
		self.matNames  = []
		self.matInfoList  = []
		self.matEntryList = []
		self.boneList = []
		self.boneMap  = []
		self.offsetList = []
		self.fvfList = []
		self.meshInfoList = []

	def loadAll(self, bs):
		self.loadMDC()
		self.loadMdlHeader(bs)

	def loadMDC(self):
		mdcFilePath = rapi.getDirForFilePath(rapi.getInputName()) + ("..\\" + "5E12DE0D\\")
		mdcFileName = mdcFilePath + rapi.getExtensionlessName(rapi.getLocalFileName(rapi.getInputName())) + ".mdc"
		if (rapi.checkFileExists(mdcFileName)):
			print(mdcFileName)
			bs = NoeBitStream(rapi.loadIntoByteArray(mdcFileName))
			self.loadMDCHeader(bs)

	def loadMDCHeader(self, bs):
		mdcHeader = bs.read("4I8f")
		print(mdcHeader)
		bs.seek(mdcHeader[0], NOESEEK_REL)
		for a in range(0, mdcHeader[1]):
			self.matEntryList.append(bs.read("5I2H7i"))
			print(self.matEntryList[a])
		matInfobase = bs.tell(); strInfoBase = matInfobase + mdcHeader[2]
		for a in range(0, mdcHeader[1]):
			bs.seek(self.matEntryList[a][0] + 0x30, NOESEEK_ABS)
			meshName = bs.readString()
			print(meshName)
			material = NoeMaterial(meshName, "")
			matBase = (self.matEntryList[a][7] + matInfobase + 0x24)
			for b in range(0, self.matEntryList[a][6]):
				bs.seek(matBase + (0xC * b), NOESEEK_ABS)
				matInfo = bs.read("2H2i")
				if matInfo[0] == 0x20:
					print("tex Data")
					bs.seek(matBase + matInfo[1], NOESEEK_ABS)
					texData = bs.read("3i")
					bs.seek(strInfoBase + matInfo[2], NOESEEK_ABS)
					slotName = bs.readString()
					bs.seek(strInfoBase + texData[0], NOESEEK_ABS)
					if texData[0] != -1:
						texName = bs.readString()
					else:
						texName = ""
					print(slotName, texName)
					if slotName == "gColorMapSampler":
						print(texName)
						material.setTexture(rapi.getLocalFileName(texName))
					elif slotName == "gNormalMapSampler":
						material.setNormalTexture(rapi.getLocalFileName(texName))
					elif slotName == "gSpecularColorMapSampler":
						material.setSpecularTexture(rapi.getLocalFileName(texName))
					elif slotName == "gEnvironmentMapSampler":
						material.setEnvTexture(rapi.getLocalFileName(texName))
				elif matInfo[0] == 0x0F:
					print("Mat Param")
					bs.seek(matBase + matInfo[1], NOESEEK_ABS)
					texData = bs.read("4f")
					bs.seek(strInfoBase + matInfo[2], NOESEEK_ABS)
					slotName = bs.readString()
					print(slotName, texData)
					if slotName == "gMaterialColor":
						material.setDiffuseColor(NoeVec4([texData[0], texData[1], texData[2], texData[3]]))
					elif slotName == "gSpecularParameter":
						material.setSpecularColor(NoeVec4([texData[0], texData[1], texData[2], texData[3]]))
					elif slotName == "gRimLightColor":
						material.setRimLighting(rimColor = NoeVec3([texData[0], texData[1], texData[2]]))
					elif slotName == "gRimLightParameter":
						material.setRimLighting(rimSize = texData[1], rimPow = texData[0], rimBias = texData[2], rimOfs = NoeVec3([0.0, texData[3], 0.0]))
			self.matList.append(material)
			self.matNames.append(meshName)


	def loadMeshNames(self, bs):
		pass
		
	def loadBones(self, bs):
		bs.seek(8, NOESEEK_REL)
		boneStart = bs.tell()
		boneNames = []
		boneParents = []
		boneMatrix1 = []
		boneMatrix2 = []
		boneMatrix3 = []
		bonePosList = []
		boneInfo = []
		for a in range(0, self.mdlInfo.boneCount):
			boneNames.append(bs.readString())
		print(boneNames)
		bs.seek(boneStart + self.mdlInfo.boneNameSize, NOESEEK_ABS)
		self.boneMapStart = bs.tell()
		bs.seek(bs.tell() + self.mdlInfo.boneMapSize, NOESEEK_ABS)
		print(bs.tell())
		for a in range(0, self.mdlInfo.boneCount):
			bs.seek(0x10, NOESEEK_REL)
			bonePos = NoeVec3.fromBytes(bs.readBytes(12))
			bonePosList.append(bonePos)
			bs.readBytes(16)
			#bs.readBytes(32) #floats
			boneInfo.append(bs.read("4hI"))			
			bs.seek(0x8, NOESEEK_REL)
		#print(bs.tell())print(bs.tell())
		print(boneInfo)
		print(bonePosList)
		for a in range(0, self.mdlInfo.boneCount):
			boneMatrix = NoeMat44.fromBytes(bs.readBytes(64)).toMat43()
			boneMatrix[3] = bonePosList[a]
			#newBone = NoeBone(a, boneNames[a], boneMatrix, None, boneInfo[a][0])
			#self.boneList.append(newBone)
		print(bs.tell())
		for a in range(0, self.mdlInfo.boneCount):
			boneMatrix = NoeMat44.fromBytes(bs.readBytes(64)).toMat43().inverse()
			boneMatrix3.append(boneMatrix)			
			print (len(boneMatrix3))
			if boneInfo[a][0] != -1:
				if boneMatrix[3] == NoeVec3((0, 0, 0)):				
					boneMatrix[3] = boneMatrix3[a - 1][3]					
			newBone = NoeBone(a, boneNames[a], boneMatrix, None, boneInfo[a][0])
			self.boneList.append(newBone)
		#if len(self.boneList) > 0:
		#	self.boneList = rapi.multiplyBones(self.boneList)
		#if len(self.boneList) > 0:
		print(self.boneList)
		print(bs.tell())
		self.loadMeshInfo(bs)
			
	def loadMeshInfo(self, bs):
		
		#if self.mdlInfo.count3 == 2:
		#	null = bs.readUInt()
		print("mdlinf")
		print(bs.tell())
		for a in range(0, self.mdlInfo.count6):
			matID, lod, matUnk01, vertCount, vertStart, faceCount, faceStart = bs.read("I2h4I")
			bs.seek(88, NOESEEK_REL)
			preName = bs.tell()
			meshName = bs.readString()
			
			if ":" in meshName:
				meshName = meshName.split(":")[0]
			
			matInfo=(matID, lod, matUnk01, vertCount, faceStart, faceCount, vertStart, meshName)
			print(matInfo)
			self.matInfoList.append(matInfo)
			bs.seek(preName + 272, NOESEEK_ABS)
		print(bs.tell())
		for a in range(0, self.mdlInfo.count7):
			self.meshInfoList.append(self.mshInfo(*bs.read("14I")))
			print(self.meshInfoList[a])		
		print(bs.tell())
		self.vertBase = bs.tell()
		bs.seek(self.mdlInfo.vertSize, NOESEEK_REL)
		print(bs.tell())
		self.faceBase = bs.tell()
		bs.seek(self.mdlInfo.faceSize, NOESEEK_REL)
		print(bs.tell())
		fvfStart = bs.tell()
		for a in range(0, self.mdlInfo.count7):
			fvfTmp = []
			fvfEnd = 0
			compCount = 0
			dataType = 0
			while fvfEnd != -1:
				fvfEnd, fvfPos, compCount, dataType, compType = bs.read("2h2Bh")
				fvfTmp.append((fvfEnd, fvfPos, compCount, dataType, compType))
			self.fvfList.append(fvfTmp)
			print(self.fvfList[a])
			
		#bs.seek(self.mdlInfo.fvfSize, NOESEEK_REL)
		print(bs.tell())
		self.loadMeshs(bs)


	def loadUnk1(self, bs):
		pass

	def loadBonePallet(self, bs):
		pass

	def loadTex(self): 
		pass

	def loadMatInfo(self, bs):
		pass

	def loadMeshs(self, bs):
		for a in range(0, self.mdlInfo.count7):
			vtxBase = self.vertBase + self.meshInfoList[a].vtxBufferStart
			idxBase = self.faceBase + self.meshInfoList[a].idxBufferStart
			vtxStride = self.meshInfoList[a].vtxStride
			matBase = self.meshInfoList[a].meshId
			fvfInfo = self.fvfList[a]
			#print(self.meshInfoList[a])
			#if len(self.matNames) > 0:
			#	rapi.rpgSetName(self.matNames[self.meshInfoList[a].matID])
			#	rapi.rpgSetMaterial(self.matNames[self.meshInfoList[a].matID])
			#else:
			#	rapi.rpgSetName(str(self.matInfoList[matBase][0]))
			rapi.rpgSetName(self.matInfoList[matBase][7])
			bs.seek(self.boneMapStart + (self.meshInfoList[a].bonePalletStart * 2), NOESEEK_ABS)
			bonePallet = bs.read(self.meshInfoList[a].bonePalletCount * "H")
			if len(bonePallet) > 0:
				rapi.rpgSetBoneMap(bonePallet)
			else:
				rapi.rpgSetBoneMap(None)

			for b in range(0, self.meshInfoList[a].meshCount):
				idxTmp = []
				matInfo = self.matInfoList[matBase + b]
				#print(matInfo)
				if len(self.matNames) > 0:
					rapi.rpgSetName(self.matNames[self.meshInfoList[a].matID] + "_" + str(a) + "_" + str(b))
				bs.seek(vtxBase + (vtxStride * matInfo[4]), NOESEEK_ABS)

				vtxBuff = bs.readBytes(vtxStride * matInfo[3])
				#print(vtxStride)
				bs.seek(idxBase, NOESEEK_ABS)
				#bs.seek(idxBase + (2 * matInfo[6]), NOESEEK_ABS)
				#print(bs.tell(), (2 * matInfo[5]), matInfo[6])
					
				for c in range(0, matInfo[5]):
					if(self.meshInfoList[a].idxType == 2):
						idxTmp.append(bs.readUInt() - matInfo[4]) # int indices
					else:
						idxTmp.append(bs.readUShort() - matInfo[4])
				idxBase = bs.tell()
				
				if matInfo[1] != 1 and matInfo[1] != -1:
					continue
				
				if(self.meshInfoList[a].idxType == 2):
					idxBuff = struct.pack("<" + 'I'*len(idxTmp), *idxTmp)
				else:
					idxBuff = struct.pack("<" + 'H'*len(idxTmp), *idxTmp)

				for c in range(0, len(fvfInfo) - 1):				
					if fvfInfo[c][4] == 0:
						rapi.rpgBindPositionBufferOfs(vtxBuff, noesis.RPGEODATA_FLOAT, vtxStride, fvfInfo[c][1])
					if fvfInfo[c][4] == 2:
						rapi.rpgBindNormalBufferOfs(vtxBuff, noesis.RPGEODATA_FLOAT, vtxStride, fvfInfo[c][1])
					if fvfInfo[c][4] == 3:
						rapi.rpgBindColorBufferOfs(vtxBuff, noesis.RPGEODATA_UBYTE, vtxStride, fvfInfo[c][1], 4)
					if fvfInfo[c][4] == 8:
						rapi.rpgBindUV1BufferOfs(vtxBuff, noesis.RPGEODATA_FLOAT, vtxStride, fvfInfo[c][1])
					if fvfInfo[c][4] == 15:
						rapi.rpgBindTangentBufferOfs(vtxBuff, noesis.RPGEODATA_FLOAT, vtxStride, fvfInfo[c][1])
					if fvfInfo[c][4] == 7:
						rapi.rpgBindBoneIndexBufferOfs(vtxBuff, noesis.RPGEODATA_UBYTE, vtxStride, fvfInfo[c][1], 4)
					if fvfInfo[c][4] == 1:
						rapi.rpgBindBoneWeightBufferOfs(vtxBuff, noesis.RPGEODATA_UBYTE, vtxStride, fvfInfo[c][1], 4)
				
				if(self.meshInfoList[a].idxType == 2):
					rapi.rpgCommitTriangles(idxBuff, noesis.RPGEODATA_UINT, matInfo[5], noesis.RPGEO_TRIANGLE, 1)
				else:
					rapi.rpgCommitTriangles(idxBuff, noesis.RPGEODATA_USHORT, matInfo[5], noesis.RPGEO_TRIANGLE, 1)
				rapi.rpgClearBufferBinds()

	def loadMdlHeader(self, bs):
		self.mdlInfo = self.mdlHeader(*bs.read("11I"))
		print(self.mdlInfo)
		self.loadBones(bs)


	# mdl Header
	mdlHeader = collections.namedtuple('mdlHeader', ' '.join((
		'boneNameSize',
		'boneMapSize',
		'boneCount',
		'count3',
		'count4',
		'count5',
		'count6',
		'count7',
		'vertSize',
		'faceSize',
		'fvfSize'		
	)))

	# mesh Info
	mshInfo = collections.namedtuple('mshInfo', ' '.join((
		'meshId',
		'meshCount',
		'vtxBufferStart',
		'vtxBufferSize',
		'vtxStride',
		'idxBufferStart',
		'idxBufferSize',
		'idxType',
		'unkOff',
		'bonePalletStart',
		'bonePalletCount',
		'matID',
		'unk02',
		'unk03'
	)))


def gsmdlLoadModel(data, mdlList):
	ctx = rapi.rpgCreateContext()
	gsmdl = gsmdlFile(NoeBitStream(data))
	gsmdl.loadAll(gsmdl.bs)
	try:
		mdl = rapi.rpgConstructModel()
	except:
		mdl = NoeModel()
	mdl.setModelMaterials(NoeModelMaterials(gsmdl.texList, gsmdl.matList))
	mdlList.append(mdl); mdl.setBones(gsmdl.boneList)	
	return 1


def gstexLoadRGBA(data, texList):
	ctx = rapi.rpgCreateContext()
	bs = NoeBitStream(data)
	imgWidth, imgHeight, type1, type2, type3, type4, unk00 = bs.read("2H4BI")
	print((imgWidth, imgHeight, type1, type2, type3, type4, unk00))
	if type3 == 0:
		if type4 == 17:
			texFmt = noesis.NOESISTEX_RGBA32
			#print("RGBA")
			dataSize = (imgWidth * imgHeight * 4)
			texData = bs.readBytes(dataSize)
			texData = rapi.imageDecodeRaw(texData, imgWidth, imgHeight, "b8g8r8p8")
			tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
			texList.append(tex1)
		else:
			texFmt = noesis.NOESISTEX_DXT1
			print("DXT1")
			dataSize = (imgWidth * imgHeight * 4) // 8
			texData = bs.readBytes(dataSize)
			tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
			texList.append(tex1)
	elif type3 == 16:
		if type4 == 17:
			texFmt = noesis.NOESISTEX_RGBA32
			#print("RGBA")
			dataSize = (imgWidth * imgHeight * 4)
			texData = bs.readBytes(dataSize)
			texData = rapi.imageDecodeRaw(texData, imgWidth, imgHeight, "b8g8r8p8")
			tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
			texList.append(tex1)
		else:
			texFmt = noesis.NOESISTEX_DXT3
			print("DXT5")
			dataSize = (imgWidth * imgHeight * 8) // 8
			texData = bs.readBytes(dataSize)
			tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
			texList.append(tex1)
	elif type3 == 32:
		texFmt = noesis.NOESISTEX_DXT5
		#print("DXT5")
		dataSize = (imgWidth * imgHeight * 8) // 8
		texData = bs.readBytes(dataSize)
		tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
		texList.append(tex1)
	elif type3 == 48:
		texFmt = noesis.NOESISTEX_RGBA32
		#print("DXT6H")
		dataSize = (imgWidth * imgHeight * 8) // 8
		texData = bs.readBytes(dataSize)
		texData = rapi.imageDecodeDXT(texData, imgWidth, imgHeight, noesis.FOURCC_BC6H)
		tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
		texList.append(tex1)
	elif type3 == 64:
		texFmt = noesis.NOESISTEX_RGBA32
		#print("DXT7")
		dataSize = (imgWidth * imgHeight * 8) // 8
		texData = bs.readBytes(dataSize)
		texData = rapi.imageDecodeDXT(texData, imgWidth, imgHeight, noesis.FOURCC_BC7)
		tex1 = NoeTexture("1", imgWidth, imgHeight, texData, texFmt)
		texList.append(tex1)

	return 1