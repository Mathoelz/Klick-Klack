using UnityEngine;
using System;
using System.Collections;


public class CableComponent : MonoBehaviour
{
    #region Class members

    [SerializeField] private PlayerController endPoint;
    [SerializeField] private PlayerController startPoint;
    [SerializeField] private Material cableMaterial;

    // Cable config
    [SerializeField] private float cableLength = 0.5f;
    [SerializeField] private int totalSegments = 5;
    [SerializeField] private float segmentsPerUnit = 2f;
    private int segments = 0;
    [SerializeField] private float cableWidth = 0.1f;

    // Solver config
    [SerializeField] private int verletIterations = 1;
    [SerializeField] private int solverIterations = 1;

    //[Range(0,3)]
    //[SerializeField] private float stiffness = 1f;

    private LineRenderer line;
    private CableParticle[] points;

    #endregion


    #region Initial setup

    void Start()
    {
        startPoint = gameObject.GetComponent<PlayerController>();
        InitCableParticles();
        InitLineRenderer();
    }

    /**
	 * Init cable particles
	 * 
	 * Creates the cable particles along the cable length
	 * and binds the start and end tips to their respective game objects.
	 */
    void InitCableParticles()
    {
        // Calculate segments to use
        if (totalSegments > 0)
            segments = totalSegments;
        else
            segments = Mathf.CeilToInt(cableLength * segmentsPerUnit);

        Vector3 cableDirection = (endPoint.transform.position - transform.position).normalized;
        float initialSegmentLength = cableLength / segments;
        points = new CableParticle[segments + 1];

        // Foreach point
        for (int pointIdx = 0; pointIdx <= segments; pointIdx++)
        {
            // Initial position
            Vector3 initialPosition = transform.position + (cableDirection * (initialSegmentLength * pointIdx));
            points[pointIdx] = new CableParticle(initialPosition);
        }

    }

    /**
	 * Initialized the line renderer
	 */
    void InitLineRenderer()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.startWidth = cableWidth;
        line.endWidth = cableWidth;
        line.positionCount = (segments + 2);
        line.material = cableMaterial;
        line.GetComponent<Renderer>().enabled = true;
    }

    #endregion


    #region Render Pass

    void Update()
    {
        RenderCable();
    }

    /**
	 * Render Cable
	 * 
	 * Update every particle position in the line renderer.
	 */
    void RenderCable()
    {
        line.SetPosition(0, startPoint.transform.position);

        for (int pointIdx = 0; pointIdx <= segments; pointIdx++)
        {
            line.SetPosition((pointIdx +1), points[pointIdx].Position);
        }

        line.SetPosition(segments + 1, endPoint.transform.position);
    }

    #endregion


    #region Verlet integration & solver pass

    void FixedUpdate()
    {

        CableParticle start = points[0];
        CableParticle end = points[segments];

        for (int verletIdx = 0; verletIdx < verletIterations; verletIdx++)
        {
            VerletIntegrate();
            SolveConstraints();
        }

        
        Vector3 Force =  end._position - endPoint.transform.position;
        endPoint.GetComponent<Rigidbody>().AddForce(Force*5);

        Force = start._position - startPoint.transform.position;
        startPoint.GetComponent<Rigidbody>().AddForce(Force*5);

    }

    /**
	 * Verler integration pass
	 * 
	 * In this step every particle updates its position and speed.
	 */
    void VerletIntegrate()
    {

        float stepping = 1.0f / (float)verletIterations;

        CableParticle start = points[0];
        CableParticle end = points[segments];
        Vector3 Force = endPoint.transform.position - end._position;
        end.ApplyPosDelta(Force * stepping);

        Force = startPoint.transform.position - start._position;
        start.ApplyPosDelta(Force * stepping);

        Vector3 gravityDisplacement = stepping* Time.fixedDeltaTime * Time.fixedDeltaTime * Physics.gravity;
        foreach (CableParticle particle in points)
        {
            particle.UpdateVerlet(gravityDisplacement);
        }
    }

    /**
	 * Constrains solver pass
	 * 
	 * In this step every constraint is addressed in sequence
	 */
    void SolveConstraints()
    {
        // For each solver iteration..
        for (int iterationIdx = 0; iterationIdx < solverIterations; iterationIdx++)
        {
            SolveDistanceConstraint();
            SolveStiffnessConstraint();
            SolveEnvConstraint();
        }
    }


    #endregion


    #region Solver Constraints

    void SolveEnvConstraint()
    {
        foreach (CableParticle particle in points)
        {
            for(int i=0; i<4; i++)
                SolveEnvConstraint(particle);
        }
    }

    /**
	 * Distance constraint for each segment / pair of particles
	 **/
    void SolveDistanceConstraint()
    {
        float segmentLength = cableLength / segments;
        for (int SegIdx = 0; SegIdx < segments; SegIdx++)
        {
            CableParticle particleA = points[SegIdx];
            CableParticle particleB = points[SegIdx + 1];

            // Solve for this pair of particles
            SolveDistanceConstraint(particleA, particleB, segmentLength);
        }
    }


    void SolveEnvConstraint(CableParticle particle)
    {
        // GroundConstraint
        RaycastHit hit;
        Vector3 d = particle._position - particle._oldPosition;
        float l = d.magnitude;
        d.Normalize();
        Ray ray = new Ray(particle._oldPosition, d);
        float EPS = 1e-5f;
        
        if (Physics.Raycast(ray, out hit, l+ EPS))
        {
            if (hit.collider.gameObject.CompareTag("Gameover"))
            {

            }
            else
            {
                Vector3 hp = hit.point;
                Vector3 cor = hp - particle._position;
                float push = Vector3.Dot(cor, hit.normal) + EPS;
                particle.Position += (push * hit.normal);
            }

            if (hit.collider.gameObject.CompareTag("Obstacle"))
            {
                hit.rigidbody.AddForceAtPosition((-1 * hit.normal) * 3, hit.point);
            }
            
        }
    }
    /**
	 * Distance Constraint 
	 * 
	 * This is the main constraint that keeps the cable particles "tied" together.
	 */
   void SolveDistanceConstraint(CableParticle particleA, CableParticle particleB, float segmentLength)
    {
        // Find current vector between particles
        Vector3 delta = particleB.Position - particleA.Position;
        // 
        float currentDistance = delta.magnitude;
        float errorFactor = 0f;
        if(currentDistance <= segmentLength)
        {
            errorFactor = currentDistance;
        }
        else
        {
            errorFactor = (currentDistance - segmentLength) / currentDistance;
        }

        // Only move free particles to satisfy constraints
        if (particleA.IsFree() && particleB.IsFree())
        {
            particleA.Position += errorFactor * 0.5f * delta;
            particleB.Position -= errorFactor * 0.5f * delta;
        }
        else if (particleA.IsFree())
        {
            particleA.Position += errorFactor * delta;
        }
        else if (particleB.IsFree())
        {
            particleB.Position -= errorFactor * delta;
        }

    }

    /**
	 * Stiffness constraint
	 **/
    void SolveStiffnessConstraint()
    {
        float distance = (points[0].Position - points[segments].Position).magnitude;
        if (distance > cableLength)
        {
            foreach (CableParticle particle in points)
            {
                SolveStiffnessConstraint(particle, distance);
            }
        }
    }

    /**
	 * TODO: I'll implement this constraint to reinforce cable stiffness 
	 * 
	 * As the system has more particles, the verlet integration aproach 
	 * may get way too loose cable simulation. This constraint is intended 
	 * to reinforce the cable stiffness.
	 * // throw new System.NotImplementedException ();
	 **/
    void SolveStiffnessConstraint(CableParticle cableParticle, float distance)
    {

    }

    #endregion
}