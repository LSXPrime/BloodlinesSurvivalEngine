using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

namespace LBSE
{
	//X Modified Version of Unity Ragdoll Builder to Auto Assign Box & Capsule Colliders & their HitSpots
	public class CreateHitSpotsWizard
	{
			private static Animator animator;
			
			private static Transform pelvis;

			private static Transform leftHips = null;

			private static Transform leftKnee = null;

			private static Transform rightHips = null;

			private static Transform rightKnee = null;

			private static Transform leftArm = null;

			private static Transform leftElbow = null;

			private static Transform rightArm = null;

			private static Transform rightElbow = null;

			private static Transform middleSpine = null;

			private static Transform head = null;
					
			private static Vector3 worldUp;

			private static ArrayList bones;

			private static CreateHitSpotsWizard.BoneInfo rootBone;

			public CreateHitSpotsWizard()
			{
			}

			private static void AddBreastColliders()
			{
				if ((middleSpine == null ? true : pelvis == null))
				{
					Bounds item = new Bounds();
					item.Encapsulate(pelvis.InverseTransformPoint(leftHips.position));
					item.Encapsulate(pelvis.InverseTransformPoint(rightHips.position));
					item.Encapsulate(pelvis.InverseTransformPoint(leftArm.position));
					item.Encapsulate(pelvis.InverseTransformPoint(rightArm.position));
					Vector3 vector3 = item.size;
					vector3[CreateHitSpotsWizard.SmallestComponent(item.size)] = vector3[CreateHitSpotsWizard.LargestComponent(item.size)] / 2f;
					BoxCollider boxCollider = Undo.AddComponent<BoxCollider>(pelvis.gameObject);
					boxCollider.isTrigger = true;
					boxCollider.center = item.center;
					boxCollider.size = vector3;
					HitSpot health = Undo.AddComponent<HitSpot>(pelvis.gameObject);
					health.DamageMultiplier = 1f;
					health.isHeadshot = false;
					health.AffectedBone = BodyBones.Spine;
					pelvis.gameObject.tag = "HitSpot";
				}
				else
				{
					Bounds bound = Clip(GetBreastBounds(pelvis), pelvis, middleSpine, false);
					BoxCollider boxCollider1 = Undo.AddComponent<BoxCollider>(pelvis.gameObject);
					boxCollider1.isTrigger = true;
					boxCollider1.center = bound.center;
					boxCollider1.size = bound.size;
					bound = Clip(GetBreastBounds(middleSpine), middleSpine, middleSpine, true);
					boxCollider1 = Undo.AddComponent<BoxCollider>(middleSpine.gameObject);
					boxCollider1.center = bound.center;
					boxCollider1.size = bound.size;
					HitSpot health = Undo.AddComponent<HitSpot>(middleSpine.gameObject);
					health.DamageMultiplier = 1f;
					health.isHeadshot = false;
					health.AffectedBone = BodyBones.Chest;
					middleSpine.gameObject.tag = "HitSpot";
				}
			}

			private static void AddHeadCollider()
			{
				int num;
				float single;
				if (head.GetComponent<Collider>())
				{
					UnityEngine.Object.Destroy(head.GetComponent<Collider>());
				}
				float single1 = Vector3.Distance(leftArm.transform.position, rightArm.transform.position);
				single1 /= 4f;
				SphereCollider sphereCollider = Undo.AddComponent<SphereCollider>(head.gameObject);
				sphereCollider.radius = single1;
				Vector3 vector3 = Vector3.zero;
				CreateHitSpotsWizard.CalculateDirection(head.InverseTransformPoint(pelvis.position), out num, out single);
				if (single <= 0f)
				{
					vector3[num] = single1;
				}
				else
				{
					vector3[num] = -single1;
				}
				sphereCollider.isTrigger = true;
				sphereCollider.center = vector3;
				HitSpot health = Undo.AddComponent<HitSpot>(head.gameObject);
				health.DamageMultiplier = 2f;
				health.isHeadshot = true;
				health.AffectedBone = BodyBones.Head;
				head.gameObject.tag = "HitSpot";
			}

			private static void AddJoint(string name, Transform anchor, string parent, Type colliderType, float radiusScale, float density)
			{
				CreateHitSpotsWizard.BoneInfo boneInfo = new CreateHitSpotsWizard.BoneInfo()
				{
					name = name,
					anchor = anchor,
					density = density,
					colliderType = colliderType,
					radiusScale = radiusScale
				};
				if (FindBone(parent) != null)
				{
					boneInfo.parent = FindBone(parent);
				}
				else if (name.StartsWith("Left"))
				{
					boneInfo.parent = FindBone(string.Concat("Left ", parent));
				}
				else if (name.StartsWith("Right"))
				{
					boneInfo.parent = FindBone(string.Concat("Right ", parent));
				}
				boneInfo.parent.children.Add(boneInfo);
				bones.Add(boneInfo);
			}

			private static void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent, Type colliderType, float radiusScale, float density)
			{
				AddJoint(string.Concat("Left ", name), leftAnchor, parent, colliderType, radiusScale, density);
				AddJoint(string.Concat("Right ", name), rightAnchor, parent, colliderType, radiusScale, density);
			}

			private static void BuildCapsules()
			{
				int num;
				float item;
				Vector3 vector3;
				foreach (CreateHitSpotsWizard.BoneInfo bone in bones)
				{
					if (bone.colliderType == typeof(CapsuleCollider))
					{
						if (bone.children.Count != 1)
						{
							Vector3 vector31 = (bone.anchor.position - bone.parent.anchor.position) + bone.anchor.position;
							CreateHitSpotsWizard.CalculateDirection(bone.anchor.InverseTransformPoint(vector31), out num, out item);
							if ((int)bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
							{
								Bounds bound = new Bounds();
								Component[] componentsInChildren = bone.anchor.GetComponentsInChildren(typeof(Transform));
								for (int i = 0; i < (int)componentsInChildren.Length; i++)
								{
									Transform transforms = (Transform)componentsInChildren[i];
									bound.Encapsulate(bone.anchor.InverseTransformPoint(transforms.position));
								}
								if (item <= 0f)
								{
									vector3 = bound.min;
									item = vector3[num];
								}
								else
								{
									vector3 = bound.max;
									item = vector3[num];
								}
							}
						}
						else
						{
							Vector3 item1 = ((CreateHitSpotsWizard.BoneInfo)bone.children[0]).anchor.position;
							CreateHitSpotsWizard.CalculateDirection(bone.anchor.InverseTransformPoint(item1), out num, out item);
						}
						CapsuleCollider capsuleCollider = Undo.AddComponent<CapsuleCollider>(bone.anchor.gameObject);
						capsuleCollider.direction = num;
						Vector3 vector32 = Vector3.zero;
						vector32[num] = item * 0.5f;
						capsuleCollider.isTrigger = true;
						capsuleCollider.center = vector32;
						capsuleCollider.height = Mathf.Abs(item);
						capsuleCollider.radius = Mathf.Abs(item * bone.radiusScale);
						HitSpot health = Undo.AddComponent<HitSpot>(bone.anchor.gameObject);
						health.DamageMultiplier = 0.35f;
						health.isHeadshot = false;
						health.AffectedBone = BodyBones.RightArm;
						bone.anchor.gameObject.tag = "HitSpot";
					}
				}
			}

			private static void CalculateDirection(Vector3 point, out int direction, out float distance)
			{
				direction = 0;
				if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
				{
					direction = 1;
				}
				if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
				{
					direction = 2;
				}
				distance = point[direction];
			}

			private static Vector3 CalculateDirectionAxis(Vector3 point)
			{
				float single;
				int num = 0;
				CalculateDirection(point, out num, out single);
				Vector3 vector = Vector3.zero;
				if (single <= 0f)
				{
					vector[num] = -1f;
				}
				else
				{
					vector[num] = 1f;
				}
				return vector;
			}
			
			private static string CheckConsistency()
			{
				string str;
				PrepareBones();
				Hashtable hashtables = new Hashtable();
				foreach (CreateHitSpotsWizard.BoneInfo bone in bones)
				{
					if (bone.anchor)
					{
						if (hashtables[bone.anchor] == null)
						{
							hashtables[bone.anchor] = bone;
						}
						else
						{
							CreateHitSpotsWizard.BoneInfo item = (CreateHitSpotsWizard.BoneInfo)hashtables[bone.anchor];
							str = string.Format("{0} and {1} may not be assigned to the same bone.", bone.name, item.name);
							return str;
						}
					}
				}
				foreach (CreateHitSpotsWizard.BoneInfo boneInfo in bones)
				{
					if (boneInfo.anchor == null)
					{
						str = string.Format("{0} has not been assigned yet.\n", boneInfo.name);
						return str;
					}
				}
				str = "";
				return str;
			}

			private static void Cleanup()
			{
				foreach (CreateHitSpotsWizard.BoneInfo bone in bones)
				{
					if (!bone.anchor)
					{
						continue;
					}
					
					Component[] componentsInChildren1 = bone.anchor.GetComponentsInChildren(typeof(Collider));
					for (int k = 0; k < (int)componentsInChildren1.Length; k++)
					{
						UnityEngine.Object.DestroyImmediate((Collider)componentsInChildren1[k]);
					}
				}
			}

			private static Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
			{
				Vector3 vector3;
				int item = CreateHitSpotsWizard.LargestComponent(bounds.size);
				if (Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.max)) > Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.min)) != below)
				{
					Vector3 vector31 = bounds.max;
					vector3 = relativeTo.InverseTransformPoint(clipTransform.position);
					vector31[item] = vector3[item];
					bounds.max = vector31;
				}
				else
				{
					Vector3 vector32 = bounds.min;
					vector3 = relativeTo.InverseTransformPoint(clipTransform.position);
					vector32[item] = vector3[item];
					bounds.min = vector32;
				}
				return bounds;
			}

			private static void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir, Vector3 outwardNormal)
			{
				outwardNormal = outwardNormal.normalized;
				normalCompo = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
				tangentCompo = outwardDir - normalCompo;
			}

			private static CreateHitSpotsWizard.BoneInfo FindBone(string name)
			{
				CreateHitSpotsWizard.BoneInfo boneInfo;
				foreach (CreateHitSpotsWizard.BoneInfo bone in bones)
				{
					if (bone.name == name)
					{
						boneInfo = bone;
						return boneInfo;
					}
				}
				boneInfo = null;
				return boneInfo;
			}

			private static Bounds GetBreastBounds(Transform relativeTo)
			{
				Bounds item = new Bounds();
				item.Encapsulate(relativeTo.InverseTransformPoint(leftHips.position));
				item.Encapsulate(relativeTo.InverseTransformPoint(rightHips.position));
				item.Encapsulate(relativeTo.InverseTransformPoint(leftArm.position));
				item.Encapsulate(relativeTo.InverseTransformPoint(rightArm.position));
				Vector3 vector3 = item.size;
				vector3[CreateHitSpotsWizard.SmallestComponent(item.size)] = vector3[CreateHitSpotsWizard.LargestComponent(item.size)] / 2f;
				item.size = vector3;
				return item;
			}

			private static int LargestComponent(Vector3 point)
			{
				int num = 0;
				if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
				{
					num = 1;
				}
				if (Mathf.Abs(point[2]) > Mathf.Abs(point[num]))
				{
					num = 2;
				}
				return num;
			}

			public static bool Create(Animator anim)
			{
				animator = anim;
				DetectBones();
				CheckConsistency();
				Cleanup();
				BuildCapsules();
				AddBreastColliders();
				AddHeadCollider();
				return true;
			}

			private static void DetectBones()
			{
				CreateHitSpotsWizard.pelvis = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.Hips);
				CreateHitSpotsWizard.leftHips = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
				CreateHitSpotsWizard.leftKnee = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
				CreateHitSpotsWizard.rightHips = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
				CreateHitSpotsWizard.rightKnee = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
				CreateHitSpotsWizard.leftArm = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
				CreateHitSpotsWizard.leftElbow = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
				CreateHitSpotsWizard.rightArm = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
				CreateHitSpotsWizard.rightElbow = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
				CreateHitSpotsWizard.middleSpine = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.Spine);
				CreateHitSpotsWizard.head = CreateHitSpotsWizard.animator.GetBoneTransform(HumanBodyBones.Head);
			}
			
			private static void PrepareBones()
			{
				worldUp = pelvis.TransformDirection(CalculateDirectionAxis(pelvis.InverseTransformPoint(head.position)));
				bones = new ArrayList();
				rootBone = new CreateHitSpotsWizard.BoneInfo()
				{
					name = "Pelvis",
					anchor = pelvis,
					parent = null,
					density = 2.5f
				};
				bones.Add(rootBone);
				AddMirroredJoint("Hips", leftHips, rightHips, "Pelvis", typeof(CapsuleCollider), 0.3f, 1.5f);
				AddMirroredJoint("Knee", leftKnee, rightKnee, "Hips", typeof(CapsuleCollider), 0.25f, 1.5f);
				AddJoint("Middle Spine", middleSpine, "Pelvis", null, 1f, 2.5f);
				AddMirroredJoint("Arm", leftArm, rightArm, "Middle Spine", typeof(CapsuleCollider), 0.25f, 1f);
				AddMirroredJoint("Elbow", leftElbow, rightElbow, "Arm", typeof(CapsuleCollider), 0.2f, 1f);
				AddJoint("Head", head, "Middle Spine", null, 1f, 1f);
			}

			private static int SecondLargestComponent(Vector3 point)
			{
				int num;
				int num1 = CreateHitSpotsWizard.SmallestComponent(point);
				int num2 = CreateHitSpotsWizard.LargestComponent(point);
				if (num1 < num2)
				{
					int num3 = num2;
					num2 = num1;
					num1 = num3;
				}
				if ((num1 != 0 ? true : num2 != 1))
				{
					num = ((num1 != 0 ? true : num2 != 2) ? 0 : 1);
				}
				else
				{
					num = 2;
				}
				return num;
			}

			private static int SmallestComponent(Vector3 point)
			{
				int num = 0;
				if (Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
				{
					num = 1;
				}
				if (Mathf.Abs(point[2]) < Mathf.Abs(point[num]))
				{
					num = 2;
				}
				return num;
			}

			private class BoneInfo
			{
				public string name;
				public Transform anchor;
				public CreateHitSpotsWizard.BoneInfo parent;
				public float radiusScale;
				public Type colliderType;
				public ArrayList children = new ArrayList();
				public float density;
				public float summedMass;
				public BoneInfo()
				{
				}
			}
	}
}
