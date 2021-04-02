/**
 * Copyright (C) 2021 haolink <https://www.twitter.com/haolink> / <https://github.com/haolink>
 * Code based on chrrox Orochi4 converter
 * 
 * Code licensed under Apache 2.0 license, see LICENSE
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

using System.Numerics;
using PMXStructure.PMXClasses;


namespace OrochiPMX
{
    public class GsmdlFile
    {
        const float SCALE = 12.5f;

        [StructLayout(LayoutKind.Sequential)]
        private struct MdlHeader
        {
            public uint BoneNameSize;
            public uint BoneMapSize;
            public uint BoneCount;
            public uint Count3;
            public uint Count4;
            public uint Count5;
            public uint Count6;
            public uint Count7;
            public uint VertSize;
            public uint FaceSize;
            public uint FvfSize;
            public uint expSize;
            public uint hasExp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Vec3
        {
            public float X;
            public float Y;
            public float Z;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BoneInfoStruct
        {
            public uint K1;
            public uint K2;
            public uint K3;
            public uint K4;
            public short S1;
            public short S2;
            public short S3;
            public short S4;
            public uint I5;
            public uint I6;
            public uint I7;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BoneInfoStructB
        {
            public uint K1;
            public uint K2;
            public short P1;
            public short P2;
            public uint K4;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MaterialInfoReadStruct
        {
            public uint MatId;
            public short Lod;
            public short MatUnk01;
            public uint VertCount;
            public uint VertStart;
            public uint FaceCount;
            public uint FaceStart;
        }

        private struct MaterialInfo
        {
            public uint MatId;
            public short Lod;
            public short MatUnk01;
            public uint VertCount;
            public uint FaceStart;
            public uint FaceCount;
            public uint VertStart;
            public string MeshName;

            public MaterialInfo(MaterialInfoReadStruct input)
            {
                this.MatId = input.MatId;
                this.Lod = input.Lod;
                this.MatUnk01 = input.MatUnk01;
                this.VertCount = input.VertCount;
                this.FaceStart = input.FaceStart;
                this.FaceCount = input.FaceCount;
                this.VertStart = input.VertStart;
                this.MeshName = "";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MeshInfo
        {
            public uint meshId;
            public uint meshCount;
            public uint vtxBufferStart;
            public uint vtxBufferSize;
            public uint vtxStride;
            public uint idxBufferStart;
            public uint idxBufferSize;
            public uint idxType;
            public uint unkOff;
            public uint bonePalletStart;
            public uint bonePalletCount;
            public uint matID;
            public uint unk02;
            public uint unk03;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FvfInfo
        {
            public short fvfEnd;
            public short fvfPos;
            public char compCount;
            public char dataType;
            public short compType;
        }


        private MdlHeader mdlInfo;

        private string inFile;
        private string mdcName;
        private string outPmx;

        private FileStream fs;
        private BinaryReader br;

        private PMXModel pmx;

        private long boneMapStart;
        private long fvfListEnd;

        private List<MaterialInfo> matInfoList;
        private List<MeshInfo> meshInfoList;
        private List<FvfInfo[]> fvfList;

        private List<MatEntryInfo> matEntryList;
        private List<string> matNames;

        private long vertBase;
        private long faceBase;

        public GsmdlFile(string inFile, string mdcName, string outPmx)
        {
            if (!File.Exists(inFile))
            {
                this.fs = null;
                return;
            }

            this.matEntryList = null;
            this.matNames = null;

            this.fs = new FileStream(inFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.br = new BinaryReader(fs);

            this.pmx = new PMXModel();
            pmx.NameEN = "New model";
            pmx.NameJP = "New model";

            this.inFile = inFile;
            this.mdcName = mdcName;
            this.outPmx = outPmx;
        }

        public void loadMdc()
        {
            if (File.Exists(this.mdcName) && this.fs != null)
            {
                this.performLoadMDC();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MDCHeader
        {
            public uint I0;
            public uint I1;
            public uint I2;
            public uint I3;
            public float f4;
            public float f5;
            public float f6;
            public float f7;
            public float f8;
            public float f9;
            public float f10;
            public float f11;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MatEntryInfo
        {
            public uint I0;
            public uint I1;
            public uint I2;
            public uint I3;
            public uint I4;
            public ushort H5;
            public ushort H6;
            public int i7;
            public int i8;
            public int i9;
            public int i10;
            public int i11;
            public int i12;
            public int i13;
        }

        private void performLoadMDC()
        {
            this.matEntryList = new List<MatEntryInfo>();
            this.matNames = new List<string>();

            int i, j;

            FileStream mfs = new FileStream(this.mdcName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader mbr = new BinaryReader(mfs);

            MDCHeader mdcHeader = mbr.ReadStruct<MDCHeader>();

            mfs.Seek(mdcHeader.I0, SeekOrigin.Current);

            for (i = 0; i < mdcHeader.I1; i++)
            {
                this.matEntryList.Add(mbr.ReadStruct<MatEntryInfo>());
            }

            long matInfoBase = mfs.Position;
            long strInfoBase = matInfoBase + mdcHeader.I2;

            for (i = 0; i < mdcHeader.I1; i++)
            {
                mfs.Seek(this.matEntryList[i].I0 + 0x30, SeekOrigin.Begin);
                string meshName = mbr.ReadASCIINullTerminatedString();
                this.matNames.Add(meshName);
            }

            mbr = null;
            mfs.Close();
            mfs = null;
        }

        public void loadMdlHeader()
        {
            if (this.fs == null)
            {
                return;
            }

            this.matInfoList = new List<MaterialInfo>();
            this.meshInfoList = new List<MeshInfo>();
            this.fvfList = new List<FvfInfo[]>();

            this.mdlInfo = this.br.ReadStruct<MdlHeader>();
            this.loadBones();
        }

        private void loadBones()
        {
            //this.fs.Seek(0x04, SeekOrigin.Current);
            long boneStart = this.fs.Position;

            List<string> boneNames = new List<string>();
            List<Vec3> bonePosList = new List<Vec3>();
            List<BoneInfoStruct> boneInfo = new List<BoneInfoStruct>();
            List<BoneInfoStructB> boneInfoB = new List<BoneInfoStructB>();
            List<Matrix4x4> boneMatrix3 = new List<Matrix4x4>();

            int i;
            for (i = 0; i < this.mdlInfo.BoneCount; i++)
            {
                boneNames.Add(this.br.ReadASCIINullTerminatedString());
            }

            this.fs.Seek(boneStart + this.mdlInfo.BoneNameSize, SeekOrigin.Begin);
            this.boneMapStart = this.fs.Position;
            this.fs.Seek(this.boneMapStart + this.mdlInfo.BoneMapSize, SeekOrigin.Begin);

            for (i = 0; i < this.mdlInfo.BoneCount; i++)
            {
                boneInfoB.Add(this.br.ReadStruct<BoneInfoStructB>());
                bonePosList.Add(this.br.ReadStruct<Vec3>());
                boneInfo.Add(this.br.ReadStruct<BoneInfoStruct>());
            }
            for (i = 0; i < this.mdlInfo.BoneCount; i++)
            {
                this.fs.Seek(64, SeekOrigin.Current);
            }

            for (i = 0; i < this.mdlInfo.BoneCount; i++)
            {
                Matrix4x4 rmatrix = br.ReadMatrix().ToMat43();
                Matrix4x4 matrix;

                Matrix4x4.Invert(rmatrix, out matrix);
                boneMatrix3.Add(matrix);

                if (boneInfo[i].S1 != -1)
                {
                    if (matrix.M41 == 0 && matrix.M42 == 0 && matrix.M43 == 0 && i > 0)
                    {
                        matrix.M41 = boneMatrix3[i - 1].M41;
                        matrix.M42 = boneMatrix3[i - 1].M42;
                        matrix.M43 = boneMatrix3[i - 1].M43;
                    }
                }

                PMXBone bone = new PMXBone(this.pmx);
                bone.NameEN = boneNames[i];
                bone.NameJP = boneNames[i];
                bone.Position.X = matrix.M41 * SCALE;
                bone.Position.Y = matrix.M42 * SCALE;
                bone.Position.Z = matrix.M43 * SCALE * (-1);

                this.pmx.Bones.Add(bone);
            }

            for (i = 0; i < boneInfoB.Count; i++)
            {
                PMXBone parent = null;
                short parentIndex = boneInfoB[i].P1;
                if (parentIndex >= 0 && parentIndex < this.pmx.Bones.Count)
                {
                    parent = this.pmx.Bones[parentIndex];
                }
                this.pmx.Bones[i].Parent = parent;
            }

            this.LoadMeshInfo();
        }

        private void LoadMeshInfo()
        {
            int i;

            for (i = 0; i < this.mdlInfo.Count6; i++)
            {
                MaterialInfoReadStruct rs = this.br.ReadStruct<MaterialInfoReadStruct>();
                this.fs.Seek(88, SeekOrigin.Current);
                long preName = this.fs.Position;
                string meshName = this.br.ReadASCIINullTerminatedString();
                MaterialInfo mi = new MaterialInfo(rs);
                mi.MeshName = meshName;
                this.matInfoList.Add(mi);
                this.fs.Seek(preName + 272, SeekOrigin.Begin);
            }
            for (i = 0; i < this.mdlInfo.Count7; i++)
            {
                this.meshInfoList.Add(this.br.ReadStruct<MeshInfo>());
            }

            this.vertBase = this.fs.Position;
            this.fs.Seek(this.mdlInfo.VertSize, SeekOrigin.Current);
            this.faceBase = this.fs.Position;
            this.fs.Seek(this.mdlInfo.FaceSize, SeekOrigin.Current);

            long fvfStart = this.fs.Position;

            for (i = 0; i < this.mdlInfo.Count7; i++)
            {
                List<FvfInfo> fvfTmp = new List<FvfInfo>();

                short fvfEnd = 0;
                while (fvfEnd != -1)
                {
                    FvfInfo fi = this.br.ReadStruct<FvfInfo>();
                    fvfEnd = fi.fvfEnd;
                    fvfTmp.Add(fi);
                }
                this.fvfList.Add(fvfTmp.ToArray());
            }

            fvfListEnd = this.fs.Position;

            this.loadMeshs();
        }

        private void loadMeshs()
        {
            int i, j, k, l, m;

            int vtxIndex = 0;

            for (i = 0; i < this.mdlInfo.Count7; i++)
            {
                long vtxBase = this.vertBase + this.meshInfoList[i].vtxBufferStart;
                long idxBase = this.faceBase + this.meshInfoList[i].idxBufferStart;
                uint vtxStride = this.meshInfoList[i].vtxStride;
                //Console.WriteLine(vtxStride);
                uint matBase = this.meshInfoList[i].meshId;
                FvfInfo[] fvfInfo = this.fvfList[i];

                this.fs.Seek(this.boneMapStart + this.meshInfoList[i].bonePalletStart * 2, SeekOrigin.Begin);
                List<ushort> bonePallet = new List<ushort>();
                for (j = 0; j < this.meshInfoList[i].bonePalletCount; j++)
                {
                    bonePallet.Add(br.ReadUInt16());
                }

                for (j = 0; j < this.meshInfoList[i].meshCount; j++)
                {
                    PMXMaterial mat = new PMXMaterial(this.pmx);
                    mat.Diffuse.R = 0.77f;
                    mat.Diffuse.G = 0.77f;
                    mat.Diffuse.B = 0.77f;
                    mat.Specular.R = 0.0f;
                    mat.Specular.G = 0.0f;
                    mat.Specular.B = 0.0f;
                    mat.Ambient.R = 0.5f;
                    mat.Ambient.G = 0.5f;
                    mat.Ambient.B = 0.5f;
                    mat.EdgeEnabled = false;
                    mat.EdgeSize = 0.0f;
                    mat.StandardToonIndex = 3;

                    string meshname = this.matInfoList[i].MeshName;

                    if (this.matNames != null && matNames.Count > 0)
                    {
                        meshname = this.matNames[(int)this.meshInfoList[i].matID] + "_" + i.ToString() + "_" + j.ToString();
                    }

                    mat.NameJP = meshname;
                    mat.NameEN = meshname;

                    MaterialInfo matInfo = this.matInfoList[(int)(matBase + j)];

                    if (matInfo.Lod != 1 && matInfo.Lod != -1)
                    {
                        continue;
                    }

                    this.fs.Seek(idxBase, SeekOrigin.Begin);
                    List<int> idxTmp = new List<int>();
                    for (l = 0; l < matInfo.FaceCount; l++)
                    {
                        if (meshInfoList[i].idxType == 2)
                        {
                            idxTmp.Add((int)(this.br.ReadUInt32() - matInfo.FaceStart));
                        }
                        else
                        {
                            idxTmp.Add((int)(this.br.ReadUInt16() - matInfo.FaceStart));
                        }
                    }
                    idxBase = this.fs.Position;


                    this.fs.Seek(vtxBase + vtxStride * matInfo.FaceStart, SeekOrigin.Begin);
                    long vtxPosition = this.fs.Position;

                    List<PMXVertex> vertices = new List<PMXVertex>();

                    for (k = 0; k < matInfo.VertCount; k++)
                    {
                        PMXVertex vtx = new PMXVertex(this.pmx);
                        byte[] idx = new byte[4] { 0, 0, 0, 0 };
                        byte[] wgt = new byte[4] { 0, 0, 0, 0 };

                        for (l = 0; l < fvfInfo.Length; l++)
                        {
                            this.fs.Seek(vtxPosition + k * vtxStride + fvfInfo[l].fvfPos, SeekOrigin.Begin);
                            switch (fvfInfo[l].compType)
                            {
                                case 0:
                                    vtx.Position.X = this.br.ReadSingle() * SCALE;
                                    vtx.Position.Y = this.br.ReadSingle() * SCALE;
                                    vtx.Position.Z = this.br.ReadSingle() * SCALE * (-1);
                                    break;
                                case 2:
                                    vtx.Normals.X = this.br.ReadSingle();
                                    vtx.Normals.Y = this.br.ReadSingle();
                                    vtx.Normals.Z = this.br.ReadSingle() * (-1);
                                    break;
                                case 8:
                                    vtx.UV.U = this.br.ReadSingle();
                                    vtx.UV.V = this.br.ReadSingle();
                                    break;
                                case 7:
                                    for (m = 0; m < 4; m++)
                                    {
                                        idx[m] = this.br.ReadByte();
                                    }
                                    break;
                                case 1:
                                    for (m = 0; m < 4; m++)
                                    {
                                        wgt[m] = this.br.ReadByte();
                                    }
                                    break;
                            }
                        }

                        int totalWeight = 0;
                        Dictionary<ushort, int> weights = new Dictionary<ushort, int>();
                        for (m = 0; m < 4; m++)
                        {
                            byte bi = idx[m];
                            if (bi >= bonePallet.Count)
                            {
                                continue;
                            }
                            ushort bni = bonePallet[bi];

                            int weight = 0;
                            if (weights.ContainsKey(bni))
                            {
                                weight = weights[bni];
                            }
                            byte cw = wgt[m];

                            totalWeight += cw;
                            weight += cw;
                            weights[bni] = weight;
                        }

                        if (weights.Count == 1)
                        {
                            PMXVertexDeformBDEF1 b1 = new PMXVertexDeformBDEF1(this.pmx, vtx);
                            b1.Bone1 = this.pmx.Bones[weights.Keys.First()];
                            vtx.Deform = b1;
                        }
                        else
                        {
                            List<KeyValuePair<ushort, int>> weightList = weights.ToList();
                            weightList.Sort(delegate (KeyValuePair<ushort, int> pair1, KeyValuePair<ushort, int> pair2)
                            {
                                return pair2.Value.CompareTo(pair1.Value);
                            });

                            for (m = 3; m >= 1; m--)
                            {
                                if (weightList.Count > m && weightList[m].Value == 0)
                                {
                                    weightList.RemoveAt(m);
                                }
                            }

                            if (weightList.Count == 1)
                            {
                                PMXVertexDeformBDEF1 b1 = new PMXVertexDeformBDEF1(this.pmx, vtx);
                                b1.Bone1 = this.pmx.Bones[weightList[0].Key];
                                vtx.Deform = b1;
                            }
                            else if (weightList.Count == 2)
                            {
                                PMXVertexDeformBDEF2 b2 = new PMXVertexDeformBDEF2(this.pmx, vtx);
                                b2.Bone1 = this.pmx.Bones[weightList[0].Key];
                                b2.Bone1Weight = (float)weightList[0].Value / (float)totalWeight;
                                b2.Bone2 = this.pmx.Bones[weightList[1].Key];
                                vtx.Deform = b2;
                            }
                            else if (weightList.Count > 2)
                            {
                                PMXVertexDeformBDEF4 b4 = new PMXVertexDeformBDEF4(this.pmx, vtx);
                                b4.Bone1 = this.pmx.Bones[weightList[0].Key];
                                b4.Bone1Weight = (float)weightList[0].Value / (float)totalWeight;
                                b4.Bone2 = this.pmx.Bones[weightList[1].Key];
                                b4.Bone2Weight = (float)weightList[1].Value / (float)totalWeight;
                                b4.Bone3 = this.pmx.Bones[weightList[2].Key];
                                b4.Bone3Weight = (float)weightList[2].Value / (float)totalWeight;
                                if (weightList.Count < 4)
                                {
                                    b4.Bone4 = null;
                                }
                                else
                                {
                                    b4.Bone4 = this.pmx.Bones[weightList[3].Key];
                                    b4.Bone4Weight = (float)weightList[3].Value / (float)totalWeight;
                                }
                                vtx.Deform = b4;
                            }
                            else
                            {
                                //Most likely a prop!
                                //Console.WriteLine("Weight error on " + this.inFile);
                                if (pmx.Bones.Count == 0)
                                {
                                    PMXBone mb = new PMXBone(pmx);
                                    mb.NameEN = "prop";
                                    mb.NameJP = "prop";
                                    mb.Position = new PMXVector3(0, 0, 0);
                                    mb.Translatable = true;
                                    pmx.Bones.Add(mb);
                                }
                                PMXVertexDeformBDEF1 b1 = new PMXVertexDeformBDEF1(this.pmx, vtx);
                                b1.Bone1 = this.pmx.Bones[0];
                                vtx.Deform = b1;
                            }
                        }

                        vertices.Add(vtx);
                        this.pmx.Vertices.Add(vtx);
                        vtxIndex++;
                    }


                    for (k = 0; k < idxTmp.Count; k += 3)
                    {
                        PMXTriangle tri = new PMXTriangle(this.pmx);
                        tri.Vertex1 = vertices[idxTmp[k + 2]];
                        tri.Vertex2 = vertices[idxTmp[k + 1]];
                        tri.Vertex3 = vertices[idxTmp[k + 0]];
                        mat.Triangles.Add(tri);
                    }
                    this.pmx.Materials.Add(mat);
                }
            }

            this.totalVertCount = (uint)vtxIndex;

            if (this.mdlInfo.hasExp == 0)
            {
                this.WritePMX();
            }
            else
            {
                this.CheckForVertexMorphs();
            }

        }

        private uint totalVertCount = 0;

        private void CheckForVertexMorphs()
        {
            int i, j, k, l;

            this.fs.Seek(this.fvfListEnd + 4, SeekOrigin.Begin);
            uint expCount = this.br.ReadUInt32();
            uint affectedMeshes = this.br.ReadUInt32();

            for (i = 0; i < expCount; i++)
            {
                string expName = this.br.ReadASCIINullTerminatedString();

                PMXMorph morph = new PMXMorph(this.pmx);
                morph.NameEN = expName;
                morph.NameJP = expName;
                morph.Panel = PMXMorph.PanelType.Other;
                this.pmx.Morphs.Add(morph);
            }

            this.fs.Seek(expCount * 4, SeekOrigin.Current);

            float dx, dy, dz;

            for (i = 0; i < affectedMeshes; i++)
            {
                int meshId = this.br.ReadInt32();
                this.fs.Seek(16, SeekOrigin.Current); //we only need the mesh id

                //Seems for these models meshId and matId is identical so let's just mix the info
                uint vtxStride = this.meshInfoList[i].vtxStride;
                uint vtxCount = this.matInfoList[i].VertCount;

                uint vtxOffset = 0;
                for (j = 0; j < meshId; j++)
                {
                    vtxOffset += this.matInfoList[j].VertCount;
                }

                for (j = 0; j < expCount; j++)
                {
                    for (k = 0; k < vtxCount; k++)
                    {
                        dx = this.br.ReadSingle() * SCALE;
                        dy = this.br.ReadSingle() * SCALE;
                        dz = this.br.ReadSingle() * SCALE * (-1);
                        this.fs.Seek(vtxStride - 8, SeekOrigin.Current);

                        if (dx != 0.0f || dy != 0.0f || dz != 0.0f)
                        {
                            PMXMorph m = this.pmx.Morphs[j];
                            PMXVertex v = this.pmx.Vertices[(int)(k + vtxOffset)];
                            PMXMorphOffsetVertex ov = new PMXMorphOffsetVertex(this.pmx, m);
                            ov.Vertex = v;
                            ov.Translation.X = dx;
                            ov.Translation.Y = dy;
                            ov.Translation.Z = dz;
                            m.Offsets.Add(ov);
                        }
                    }
                }
            }


            /*PMXMorph morph = new PMXMorph(this.pmx);
            morph.NameJP = expressionNames[0];
            morph.NameEN = expressionNames[0];

            long offset = fs.Position - this.fvfListEnd;

            Console.WriteLine(expCount);
            Console.WriteLine(totalVertCount * 72);
            Console.WriteLine(totalVertCount * 72 * expCount);
            Console.WriteLine(this.mdlInfo.expSize.ToString("X8").ToUpper());
            Console.WriteLine((this.fvfListEnd + this.mdlInfo.expSize).ToString("X8"));

            this.fs.Seek(0x100, SeekOrigin.Current);

            uint x = 0;
            
            while (br.ReadByte() == 0)
            {
                x++;
            }
            Console.WriteLine(x);
            Console.WriteLine(fs.Position.ToString("X8"));*/

            /*float dx, dy, dz;
            for (i = 0; i < totalVertCount; i++)
            {
                dx = br.ReadSingle() * SCALE;
                dy = br.ReadSingle() * SCALE;
                dz = br.ReadSingle() * SCALE * (-1);

                if (dx != 0.0f || dy != 0.0f || dz != 0.0f)
                {
                    PMXMorphOffsetVertex dvtx = new PMXMorphOffsetVertex(this.pmx, morph);
                    dvtx.Vertex = this.pmx.Vertices[i];
                    dvtx.Translation.X = dx;
                    dvtx.Translation.Y = dy;
                    dvtx.Translation.Z = dz;
                    morph.Offsets.Add(dvtx);
                }
                this.fs.Seek(60, SeekOrigin.Current);
            }*/

            //this.pmx.Morphs.Add(morph);

            this.WritePMX();
        }

        private void WritePMX()
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.outPmx)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.outPmx));
            }

            this.pmx.SaveToFile(this.outPmx);
        }
    }
}
