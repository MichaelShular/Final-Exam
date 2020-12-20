using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] cubes;
    public BulletBehaviour[] spheres;

    private static Vector3[] faces;

    // Start is called before the first frame update
    void Start()
    {
        cubes = FindObjectsOfType<CubeBehaviour>();

        faces = new Vector3[]
        {
            Vector3.left, Vector3.right,
            Vector3.down, Vector3.up,
            Vector3.back , Vector3.forward
        };
    }

    // Update is called once per frame
    void Update()
    {
        spheres = FindObjectsOfType<BulletBehaviour>();

        // check each AABB with every other AABB in the scene
        for (int i = 0; i < cubes.Length; i++)
        {
            for (int j = 0; j < cubes.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(cubes[i], cubes[j]);
                }
            }
        }

        // Check each sphere against each AABB in the scene
        foreach (var sphere in spheres)
        {
            foreach (var cube in cubes)
            {
                if (cube.name != "Player")
                {
                    CheckSphereAABB(sphere, cube);
                }
                
            }
        }


    }

    public static void CheckSphereAABB(BulletBehaviour s, CubeBehaviour b)
    {
        //// get box closest point to sphere center by clamping
        //var x = Mathf.Max(b.min.x, Mathf.Min(s.transform.position.x, b.max.x));
        //var y = Mathf.Max(b.min.y, Mathf.Min(s.transform.position.y, b.max.y));
        //var z = Mathf.Max(b.min.z, Mathf.Min(s.transform.position.z, b.max.z));

        //var distance = Math.Sqrt((x - s.transform.position.x) * (x - s.transform.position.x) +
        //                         (y - s.transform.position.y) * (y - s.transform.position.y) +
        //                         (z - s.transform.position.z) * (z - s.transform.position.z));

        Contact contactB = new Contact(b);

        if ((s.min.x <= b.max.x && s.max.x >= b.min.x) &&
            (s.min.y <= b.max.y && s.max.y >= b.min.y) &&
            (s.min.z <= b.max.z && s.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - s.min.x),
                (s.max.x - b.min.x),
                (b.max.y - s.min.y),
                (s.max.y - b.min.y),
                (b.max.z - s.min.z),
                (s.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }

            s.penetration = penetration;
            s.collisionNormal = face;
            //s.isColliding = true;

            if (!s.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = s.contacts.Count - 1; i > -1; i--)
                {
                    if (s.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        s.contacts.RemoveAt(i);
                    }
                }
                Reflect(s, b);
            }
        }

    }
    
    // This helper function reflects the bullet when it hits an AABB face
    private static void Reflect(BulletBehaviour s, CubeBehaviour b)
    {
        if ((s.collisionNormal == Vector3.forward))
        {
            s.transform.position += new Vector3(0, 0, -0.05f);
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
            s.isColliding = false;
        }
        else if ((s.collisionNormal == Vector3.back))
        {
            s.transform.position += new Vector3(0, 0, 0.05f);
            s.direction = new Vector3(s.direction.x, s.direction.y, -s.direction.z);
            s.isColliding = false;
        }
        else if ((s.collisionNormal == Vector3.right))
        {
            s.transform.position += new Vector3(-0.05f, 0, 0);
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
            s.isColliding = false;
        }
        else if ((s.collisionNormal == Vector3.left))
        {
            s.transform.position += new Vector3(0.05f, 0, 0);
            s.direction = new Vector3(-s.direction.x, s.direction.y, s.direction.z);
            s.isColliding = false;
        }
        else if ((s.collisionNormal == Vector3.up))
        {
            s.transform.position += new Vector3(0, -0.01f, 0); 
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
            s.isColliding = false;
        }
        else if ((s.collisionNormal == Vector3.down))
        {
            s.transform.position += new Vector3(0, 0.01f, 0);
            s.direction = new Vector3(s.direction.x, -s.direction.y, s.direction.z);
            s.isColliding = false;
        }
    }


    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        Contact contactB = new Contact(b);

        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            // determine the distances between the contact extents
            float[] distances = {
                (b.max.x - a.min.x),
                (a.max.x - b.min.x),
                (b.max.y - a.min.y),
                (a.max.y - b.min.y),
                (b.max.z - a.min.z),
                (a.max.z - b.min.z)
            };

            float penetration = float.MaxValue;
            Vector3 face = Vector3.zero;

            // check each face to see if it is the one that connected
            for (int i = 0; i < 6; i++)
            {
                if (distances[i] < penetration)
                {
                    // determine the penetration distance
                    penetration = distances[i];
                    face = faces[i];
                }
            }
            
            // set the contact properties
            contactB.face = face;
            contactB.penetration = penetration;


            // check if contact does not exist
            if (!a.contacts.Contains(contactB))
            {
                // remove any contact that matches the name but not other parameters
                for (int i = a.contacts.Count - 1; i > -1; i--)
                {
                    if (a.contacts[i].cube.name.Equals(contactB.cube.name))
                    {
                        a.contacts.RemoveAt(i);
                    }
                }
                if (a.name == "Player" && b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {
                   
                    if (contactB.face == Vector3.right)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.08f + (1.2f * penetration), 0.0f, 0.0f);
                        Vector3 q = new Vector3(0.0f, 0.07f, 0.0f);
                        b.transform.position += i;
                        b.transform.position += q;
                       
                    }
                    if (contactB.face == Vector3.left )
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(-0.08f - (1.2f * penetration), 0.0f, 0.0f);
                        Vector3 q = new Vector3(0.0f, 0.09f, 0.0f);
                        b.transform.position += i;
                        b.transform.position += q;

                    }
                    if (contactB.face == Vector3.back)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.0f, 0.0f, -0.08f - (1.2f * penetration));
                        Vector3 q = new Vector3(0.0f, 0.09f, 0.0f);
                        b.transform.position += i;
                        b.transform.position += q;
                        

                    }
                    if (contactB.face == Vector3.forward)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.0f, 0.0f, 0.08f + (1.2f * penetration));
                        Vector3 q = new Vector3(0.0f, 0.07f, 0.0f);
                        b.transform.position += i;
                        b.transform.position += q;
                
                    }
                }
                if (a.name == "Player" && b.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.STATIC)
                {
                    if (contactB.face == Vector3.right)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.05f + (1.1f * penetration), 0.0f, 0.0f);
                        a.transform.position -= i;                  
                    }
                    if (contactB.face == Vector3.left)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(-0.05f - (1.1f * penetration), 0.0f, 0.0f);
                        a.transform.position -= i;
                    }
                    if (contactB.face == Vector3.back)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.0f, 0.0f, -0.05f - (1.1f * penetration));
                        a.transform.position -= i;
                    }
                    if (contactB.face == Vector3.forward)
                    {
                        a.gameObject.GetComponent<RigidBody3D>().Stop();
                        Vector3 i = new Vector3(0.0f, 0.0f, 0.05f + (1.1f * penetration));
                        a.transform.position -= i;

                    }
                }

                    if (contactB.face == Vector3.down)
                {
                    a.gameObject.GetComponent<RigidBody3D>().Stop();
                    a.isGrounded = true;
                }

                
                // add the new contact
                a.contacts.Add(contactB);
                a.isColliding = true;
               
            }
        }
        else
        {

            if (a.contacts.Exists(x => x.cube.gameObject.name == b.gameObject.name))
            {
                a.contacts.Remove(a.contacts.Find(x => x.cube.gameObject.name.Equals(b.gameObject.name)));
                a.isColliding = false;

                if (a.gameObject.GetComponent<RigidBody3D>().bodyType == BodyType.DYNAMIC)
                {

                    a.gameObject.GetComponent<RigidBody3D>().isFalling = true;
                    a.isGrounded = false;
                   
                }
            }
        }
    }
}
