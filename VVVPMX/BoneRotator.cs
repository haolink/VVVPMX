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

using System.Windows.Media.Media3D;
using PMXStructure.PMXClasses;

namespace VVVPMX
{
    public static class Vector3Extensions
    {
        public static Point3D ToPoint3D(this PMXVector3 v)
        {
            return new Point3D(v.X, v.Y, v.Z);
        }

        public static PMXVector3 ToPMXVector3(this Point3D p)
        {
            return new PMXVector3((float)(p.X), (float)(p.Y), (float)(p.Z));
        }
    }    

    class BoneRotator
    {        
        public static void RotateZ(PMXModel mdl, PMXBone parent, double angle)
        {
            List<PMXBone> children = GetAllChildBones(mdl, parent);

            foreach (PMXBone child in children)
            {
                PMXVector3 originalLocation = child.Position;
                child.Position = Rotate(parent.Position, originalLocation, angle, 1.0f);

                if (!child.HasChildBone)
                {
                    PMXVector3 targetLocation = Rotate(parent.Position, originalLocation + child.ChildVector, angle, 1.0f);
                    child.ChildVector = targetLocation - child.Position;
                }
            }

            List<PMXBone> weightBones = new List<PMXBone>() { parent };
            weightBones.AddRange(children);

            PMXVector3 baseVector = new PMXVector3();

            foreach (PMXVertex v in mdl.Vertices)
            {
                float w = DetermineWeight(v, weightBones);

                if (w <= 0.0f)
                {
                    continue;
                }                

                PMXVector3 originalLocation = v.Position;

                v.Position = Rotate(parent.Position, originalLocation, angle, w);
                v.Normals = Rotate(baseVector, v.Normals, angle, w);
            }
        }    

        private static void AddWeightInternal(PMXBone bne, float weight, Dictionary<PMXBone, float> weights, ref float weightSum)
        {
            if (weight == 0 || bne == null)
            {
                return;
            }

            if(!weights.ContainsKey(bne))
            {
                weights[bne] = 0.0f;
            }

            weights[bne] += weight;
            weightSum += weight;
        }

        private static float DetermineWeight(PMXVertex v, List<PMXBone> checkBones)
        {
            Dictionary<PMXBone, float> weights = new Dictionary<PMXBone, float>();
            float weightSum = 0.0f;

            if (v.Deform is PMXVertexDeformBDEF4)
            {
                PMXVertexDeformBDEF4 df4 = (PMXVertexDeformBDEF4)(v.Deform);
                AddWeightInternal(df4.Bone1, df4.Bone1Weight, weights, ref weightSum);
                AddWeightInternal(df4.Bone2, df4.Bone2Weight, weights, ref weightSum);
                AddWeightInternal(df4.Bone3, df4.Bone3Weight, weights, ref weightSum);
                AddWeightInternal(df4.Bone4, df4.Bone4Weight, weights, ref weightSum);
            }
            else if (v.Deform is PMXVertexDeformBDEF2)
            {
                PMXVertexDeformBDEF2 df2 = (PMXVertexDeformBDEF2)(v.Deform);
                AddWeightInternal(df2.Bone1, df2.Bone1Weight, weights, ref weightSum);
                AddWeightInternal(df2.Bone2, 1.0f - df2.Bone1Weight, weights, ref weightSum);
            }
            else
            {
                PMXVertexDeformBDEF1 df1 = (PMXVertexDeformBDEF1)(v.Deform);
                AddWeightInternal(df1.Bone1, 1.0f, weights, ref weightSum);
            }

            if(weights.Count == 0 || weightSum <= 0.0f)
            {
                return 0.0f;
            }

            float resWeight = 0.0f;
            foreach (PMXBone b in checkBones)
            {
                if (weights.ContainsKey(b)) {
                    resWeight += weights[b];
                }
            }

            if (resWeight > 1.0f)
            {
                return 1.0f;
            }
            else if(resWeight < 0.0f)
            {
                return 0.0f;
            }
            return resWeight;
        }
        
        private static PMXVector3 Rotate(PMXVector3 centre, PMXVector3 point, double angle, float weight)
        {
            PMXVector3 offset = point - centre;

            if (offset.Value == 0)
            {
                return point;
            }

            RotateTransform3D xrotation = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle)); //WPF for whatever reasons uses degrees and not Radian

            PMXVector3 rotated = xrotation.Transform(offset.ToPoint3D()).ToPMXVector3();

            PMXVector3 actualRotation = (rotated - offset) * weight + offset;

            return centre + actualRotation;
        }

        public static void StraightenHierarchie(PMXModel mdl, PMXBone source)
        {
            List<PMXBone> bones = new List<PMXBone>() { source };
            bones.AddRange(GetAllChildBones(mdl, source));

            foreach (PMXBone b in bones)
            {
                b.CreateLocalCoodinateAxis();
            }
        }

        public static List<PMXBone> GetAllChildBones(PMXModel mdl, PMXBone parent)
        {
            List<PMXBone> res = new List<PMXBone>();

            foreach (PMXBone b in mdl.Bones)
            {
                if (b.Parent != parent)
                {
                    continue;
                }

                res.Add(b);
                res.AddRange(GetAllChildBones(mdl, b));
            }

            return res;
        }
    }
}
