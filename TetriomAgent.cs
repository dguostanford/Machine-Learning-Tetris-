using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MLAgents; 


// Don't use "System.Collections.Generic" if you want to export your app to flash

public class Tetrimo : MonoBehavior {
    #region Shapes and Colors
    static Vector2[, ,] Shapes = new Vector2[7,4,4] {
        // #
        // This structure is used in some places to represent 2D positions and vectors (e.g. texture coordinates in a Mesh or texture offsets in Material). In the majority of other cases a Vector3 is used. // Static Properties
		// down	Shorthand for writing Vector2(0, -1).
		// left	Shorthand for writing Vector2(-1, 0).
		// negativeInfinity	Shorthand for writing Vector2(float.NegativeInfinity, float.NegativeInfinity).
		// one	Shorthand for writing Vector2(1, 1).
		// positiveInfinity	Shorthand for writing Vector2(float.PositiveInfinity, float.PositiveInfinity).
		// right	Shorthand for writing Vector2(1, 0).
		// up	Shorthand for writing Vector2(0, 1).
		// zero	Shorthand for writing Vector2(0, 0).
		{
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0) },
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0,-1) },
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 0) },
            {new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1), new Vector2(0,-1) },
        },
        // code instantiates/declares a new vector2 structure at the specific position of the Vector2. the vector2 merely stores the values of the position for the block 

        // ###
		{
            {new Vector2(2, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(1,-1)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1)}
        },
        //  #
        // ###
		{
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 0)},
            {new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1)}
        },
        // ##
        // ##
		{
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)}
        },
        //
        // ####

        //straight line 
		{
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(0,-2)},
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(0,-2)}
        },                                                                           
        //  ##                                                                       
        // ##                                                                        
		{                                                                            
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1,-1)},
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1,-1)}
        },                                                                           
		// ##                                                                        
        //  ##                                                                       
		{                                                                            
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,-1)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,-1)}
        }
	};
    static Color[] Colors = new Color[7] { //whenever a new piece arrives, you create it according to its color. not every vector2 has the same position, which is because
    	//none share the same exact piece
        Color.red,
        Color.green,
        Color.blue,
        Color.white,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };
    #endregion
    #region NestedTypes
    public struct CFieldSize { //within the struct of the field size, you can either be falling, landing, or fixed. these are variables that are instantiated with 
    	//the fact of the field size of an object. the field size could be added onto the blocks when they are fixed, which is an item that becomes true when the falling 
    	//down has stopped 
        public int Left, Right, Bottom, Top;
    }
    /// <summary>
    /// <list type="">
    ///     <item>Falling</item>
    ///     <item>Landed </item>
    ///     <item>Fixed< /item>
    /// </list>
    /// </summary>
    public enum TetrimoState { //the state can be either in these 5 different states, these states determine its qualities. a struct that creates the certain packages
    	//and things we can infer about the tetrimo based on its state 
        Spawning,   // Tetrimo is about to become "Falling" 
        Falling,
        Landed,
        Fixed,
        Preview
    }
    #endregion
// this instantiates all the data types, such as the 
    #region Class instances data members 
    /// <summary>All tetrimos in Preview State</summary>
    public static Tetrimo NextTetrimo;
    public static uint TetrimoCount = 1;
    /// <summary>border position of the game field (inclusive values)</summary>
    static CFieldSize FieldSize = new CFieldSize() { // new field size that is around this size
        Left    =   0,
        Right   =  10,
        Top     =  18,  // always start at top-1, cause the center of each tetrimo is the second element on the lower row
        Bottom  =   0
    };
    /// <summary>Start position of new Tetrimos in 'Preview' state</summary>
    static Vector2 PreviewHUD = new Vector2() {
        x = 16,
        y = 16
    };
    /// <summary>
    /// <list type="">
    ///     <item>null,         if position is free</item>
    ///     <item>gameObject,   if position is occupied </item>
    /// </list>
    /// </summary>
    static GameObject[,] FieldMatrix = new GameObject[19, 11];
    #endregion

    #region Fields (Set in Unity Editor)
    /// <summary>Pointer to itselfs => create new Tetrimos</summary>
    public GameObject   TetrimoPrefab;
    public GameObject   TetrimoPartPrefab;
    public GameObject   TetrimoExplosionPrefab;
    public GameObject   FourLinesLabelPrefab;

    public TetrimoState State;
    /// <summary>Swap action cooldown</summary>
    public float        SwapCooldown;
    /// <summary>Horizontal movement speed</summary>
    public float        HorizontalSpeed;
    public float        FallingCooldown;
    public float        FallingSpeed;
    #endregion

    #region Private Fields
    /// <summary>Countdown for next swap</summary>
    float NextSwap = 0.0f;
    /// <summary>Countdown for next fall</summary>
    float NextFall;
    int   ShapeIndex, RotationIndex;
    bool  IsMovingHorizontal = false;
    #endregion

    #region Properties
    bool CanMoveDown {
        get {
            if(IsMovingHorizontal)
                return false;

            foreach (Transform child in transform) {
                if (Mathf.RoundToInt(child.position.y - 1) < 0 || FieldMatrix[Mathf.RoundToInt(child.position.y - 1), Mathf.RoundToInt(child.position.x)] != null)
                    return false;
            }
            return true;
        }
    }
    bool CanMoveRight { // if the position of the shape itself is less than or equal to the size of the field to the right of the space, then you can move right, 
    	//as long as the current position as true and all position transformations are true as well 
        get {
            bool canMoveRight = true;
            foreach (Transform child in transform) {
                canMoveRight &= Mathf.RoundToInt(child.position.x + 1) <= Tetrimo.FieldSize.Right && FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x + 1)] == null;
            }
            return canMoveRight;
        }
    }
    bool CanMoveLeft {
        get {
            bool canMoveLeft = true;
            foreach (Transform child in transform) {
                canMoveLeft &= Mathf.RoundToInt(child.position.x - 1) >= Tetrimo.FieldSize.Left && FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x - 1)] == null;
            }
            return canMoveLeft;
        }
    }
    bool CanRotate { //create a new vector called tmp that has this vector2 position that is added into the shape and rotationdex of it 
        get {
            // Iterate through each TetrimoParts
            for (int index = 0; index < Shapes.GetLength(2); index++) {
                Vector2 tmp = new Vector2(transform.position.x, transform.position.y) + Shapes[ShapeIndex, (RotationIndex + 1) % Shapes.GetLength(2), index];

                if (tmp.x < FieldSize.Left || tmp.x > FieldSize.Right || tmp.y < FieldSize.Bottom || tmp.y > FieldSize.Top)
                    return false;

                if (FieldMatrix[Mathf.RoundToInt(tmp.y), Mathf.RoundToInt(tmp.x)] != null)
                    return false;
            }
            return true;
        }
    }
    #endregion

    #region Event Handler
    void Start () {
        switch (State) {
        case TetrimoState.Spawning: // when the start of the game is made, you automatically start a spawning tetrimo 
            NextFall = FallingCooldown;
            AgentAction();

            // Create shape and translate it at the center top of the game field.
            // REMARK: Do not use translate here! Instantiate create a copy of the current object.
            CreateShape();
            transform.position = new Vector3((int)(FieldSize.Right / 2), FieldSize.Top - 1, 0); //you create a shape with the 3D position of the field size to the right/2, etc.

            //3D position that is transformed to the top 

            // Check if the player has lost the game
            foreach (Transform child in transform) {
                if (FieldMatrix[(int)child.position.y, (int)child.position.x] != null) { //if the field matrx position of y and x is not equal to the null, meaning that 
                	//there is something at these positions, then that means that the game has ended 
                    AgentAction();
                    Application.LoadLevel("GameOver");
                }
            }
            name = "Tetrimo#" + (TetrimoCount-1);
            State = TetrimoState.Falling;
            break;

        case TetrimoState.Preview:
            RotationIndex = Random.Range(0, Shapes.GetLength(1));
            ShapeIndex    = Random.Range(0, Shapes.GetLength(0));
            CreateShape();
            transform.position = new Vector3(PreviewHUD.x, PreviewHUD.y);

            NextTetrimo = this;

            TetrimoCount++;
            name = "Tetrimo#" + TetrimoCount;
            break;
        }
	}
    void Update() { //if it's not at the bottom and it's not in preview and it's not spawning, it's falling down or landing 
        if (State != TetrimoState.Fixed && State != TetrimoState.Preview && State != TetrimoState.Spawning) {
            if (State != TetrimoState.Landed && Input.GetAxis("Vertical") < 0)
                StartCoroutine(FallingDown());

            if (Input.GetButtonDown("Horizontal"))
                StartCoroutine(MoveHorizontal());

            // Set "up" as alternative button for Jump (Project => Input)
            if ((Input.GetButton("Jump")) && Time.time > NextSwap) {    
                if (this.CanRotate) {
                    StartCoroutine(RotateTetrimo());
                    NextSwap = Time.time + SwapCooldown;
                    AgentAction();
                }
            }

            // Automatic falling down
            if (NextFall < 0) {
                StartCoroutine(FallingDown());
                NextFall = FallingCooldown;
                AgentAction();
            }
            NextFall -= FallingSpeed * Time.deltaTime;
        }

        if (State == TetrimoState.Preview) {
            transform.Rotate(0, 1f, 0, Space.World);

            AgentAction();
            collectObservations()
        }

        // Debugging Feature
        if (Input.GetKeyDown(KeyCode.F2)) {
            KillAndReload();
        }
    }
    #endregion

    #region Coroutines: http://unitygems.com/coroutines/
    /// <summary>
    /// Active the Tetrimo in the preview hub
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivateTetrimoInPreview() {
        yield return 0;
    }
    /// <summary>
    /// Create a new Tetrimo and place it in the preview hub.
    /// </summary>
    /// <returns></returns>
    IEnumerator CreatePreviewStateTetrimo() {
        yield return 0;
    }
    IEnumerator MoveHorizontal() {
        IsMovingHorizontal = true;

        float moved     = 0.0f;
        float direction = Input.GetAxis("Horizontal");

        if ((this.CanMoveRight && direction > 0) || (this.CanMoveLeft && direction < 0)) { //can move left is based on the field matrix not being 0 when trying to move 
            while (moved <= 1.0f) {
                float moveStep = Mathf.Min(HorizontalSpeed * Time.deltaTime, 1.1f - moved);   // 1.1f since float has some rounding problems!

                if (direction > 0)
                    transform.Translate(Vector3.right * moveStep, Space.World);
                else if (direction < 0)
                    transform.Translate(Vector3.left  * moveStep, Space.World);

                moved += moveStep;
                yield return 0;
            }

            // We will correct the actual position of each stone when it landed
            transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }

        IsMovingHorizontal = false;
    }


    IEnumerator FallingDown() { //transform refers to unity script transformation that each separate entity shape has a fixed number of children orientations and forms.
        if (this.CanMoveDown) {
            transform.Translate(Vector3.down, Space.World);
        }
        else {
            if (State == TetrimoState.Falling) {
                // After the Tetrimo has landed, the player can move it in the first 400ms.
                this.GetComponent<AudioSource>().Play();
                State = TetrimoState.Landed;
                yield return new WaitForSeconds(0.4f);
                while (IsMovingHorizontal)  // Wait for end of possible MoveHorizontal - calls
                    yield return new WaitForEndOfFrame();

                if (this.CanMoveDown) {
                    State = TetrimoState.Falling;
                }
                else {
                    State = TetrimoState.Fixed; //after it is done falling, meaning the player has been able to have it reach the bottom and the user has successfully been able to move the shape all the way


                    foreach (Transform child in transform) { //field matrix refers to the field that the shape operates within 
                        Tetrimo.FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x)] = child.gameObject;
                    }
                    ArrayList lines = FindLines();
                    if (lines.Count > 0) { //lines means how many lines you've been able to score, hence the line bonus it talks about 
                        int FourLineBonus = 0;
                        collectObservations();



                        //destroying a block when you score a bonus

                        // Destruction animation
                        foreach (int line in lines) {
                            int y = Mathf.CeilToInt(line);

                            for (int i = FieldSize.Left; i <= FieldSize.Right; i++) {
                            	//dicated by field size
                                Instantiate(TetrimoExplosionPrefab, FieldMatrix[y, i].transform.position, Quaternion.identity);
                                Destroy(FieldMatrix[y, i]);
                                FieldMatrix[y, i] = null; //you destroy a fieldmatrix/delete size when the value at that position is null. when a block occupies it,
                                //it becomes not null, therefore meaning that other different blocks will be able to fall and become fixed on the position 
                                
                                AgentAction();
                                
                            }
                        }
                        if (lines.Count == 4) {
                            FourLineBonus = FieldMatrix.GetLength(1);
                            Instantiate(FourLinesLabelPrefab, new Vector3(transform.position.x, transform.position.y + 1.0f), Quaternion.identity);

                            AgentAction();


                        }


                        // update logic
                        yield return new WaitForEndOfFrame();
                        lines.Sort();
                        lines.Reverse();
                        foreach (int line in lines) {
                            if (line >= FieldSize.Top) // if the game hasn't ended e.g. the pieces have reached the top 
                                continue;
                            LetLinesAboveFalling(line + 1);
                            foreach (int line in lines) {
                                AgentAction();
                                addReward(-0.51 * lines.FloorToInt + 0.76 * Bumpiness - 0.36 * aggregateHeight - 0.18 * Bumpiness));
                                
                            }
                            }

                        }

                        // adding scores
                        MatchCamera.Scores += FieldMatrix.GetLength(1) * lines.Count + MatchCamera.Continuous + FourLineBonus;
                        MatchCamera.Continuous++;

                        // check if next level has been reached
                        if (MatchCamera.Scores >= MatchCamera.Level * MatchCamera.ScoresForNextLevel) {
                            MatchCamera.Level = 1 + Mathf.FloorToInt(((float)MatchCamera.Scores) / MatchCamera.ScoresForNextLevel);
                            AgentAction();
                            FallingSpeed++;
                        }
                    else {
                        MatchCamera.Continuous = 0;
                    }

                    // Create the new Tetrimo as previewed by destroying the previewed Tetrimo, and place a new created Tetrimo
                    // yield return new WaitForSeconds(0.5f);
                    ActivateAndCreateNewPreview();

                    // Extract children (TetrimoParts) from Tetrimo. We will delete the Tetrimo itself.
                    Transform[] children = GetComponentsInChildren<Transform>();
                    for (int i = 0; i < children.Length; i++)
                        children[i].parent = null;
                    Destroy(gameObject);
                }
            }
        }
    }






    void ActivateAndCreateNewPreview() {
        GameObject newGameObject = (GameObject)Instantiate(TetrimoPrefab, Vector3.zero, Quaternion.identity);
        Tetrimo newFallingTetrimo       = newGameObject.GetComponent<Tetrimo>();
        newFallingTetrimo.RotationIndex = NextTetrimo.RotationIndex;
        newFallingTetrimo.ShapeIndex    = NextTetrimo.ShapeIndex;
        newFallingTetrimo.State         = TetrimoState.Spawning;

        foreach (Transform child in newFallingTetrimo.transform) {
            Destroy(child.gameObject);
        }
        Destroy(NextTetrimo.gameObject);

        newGameObject = (GameObject)Instantiate(TetrimoPrefab, Vector3.zero, Quaternion.identity);
        Tetrimo newPreviewTetrimo = newGameObject.GetComponent<Tetrimo>();
        newPreviewTetrimo.State = TetrimoState.Preview;
    }


    IEnumerator RotateTetrimo() {
        this.GetComponent<AudioSource>().Play();

        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        RotationIndex = (RotationIndex + 1) % Shapes.GetLength(2);
        CreateShape();
        yield return 0;

        AgentAction();
        collectObservations();
    }


    void LetLinesAboveFalling(float lineToBegin) {
        int bottom = Mathf.RoundToInt(lineToBegin);

        for (int y = bottom; y <= FieldSize.Top; y++) {
            for (int x = FieldSize.Left; x <= FieldSize.Right; x++) {
                FieldMatrix[y-1, x] = FieldMatrix[y, x];
                if (FieldMatrix[y-1, x] != null)
                    FieldMatrix[y-1, x].transform.Translate(Vector3.down);

                FieldMatrix[y, x] = null;
            }
        }
    }
    #endregion

    #region Helper functions
    void CreateShape() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < Shapes.GetLength(2); index++) {
            GameObject tmp = (GameObject)Instantiate(TetrimoPartPrefab, Shapes[ShapeIndex, RotationIndex, index], Quaternion.identity);

            tmp.transform.Translate(transform.position);
            tmp.transform.parent = gameObject.transform;
            tmp.GetComponent<Renderer>().material.color = Colors[ShapeIndex];
        }
    }
    /// <summary>
    /// Check matrix for full filled lines, where the current Tetrimo is part of it.
    /// </summary>
    /// <returns>y-indices of type int</returns>
    ArrayList FindLines() {
        ArrayList lines = new ArrayList();
        Hashtable open  = new Hashtable(); // Never use classes or methods, that aren't listened in the Unity reference, if you aim for cross-plattform-support.

        foreach (Transform child in transform) {
            int line = Mathf.CeilToInt(child.position.y);
            if(!open.ContainsKey(line))
                open.Add(line, line);
        }

        foreach (int y in open.Keys) {
            bool fullline = true;
            for (int i = FieldSize.Left; i <= FieldSize.Right; i++)
                fullline &= FieldMatrix[Mathf.RoundToInt(y), i] != null;
            if (fullline)
                lines.Add((int) y);
        }
        return lines;
    }

    public override void collectObservations() { //collect needed variables for a fitness function 
        Vector2 Height = lines.FloorToInt;
        Vector2 Lines = lines.FindLines;
        Vector2 Bumpiness = Height/lines.FindLines;
        Vector2 aggregateHeight = fieldmatrix.GetLength;
        vector2 aggregateHeightAxis = fieldmatrix.GetAxis;

        addObservations(this.Bumpiness.x);
        addObservations(this.Bumpiness.FloorToInt.x);
        addObservations(this.aggregateHeight);
        addObservations(this.aggregateHeightAxis);

        addReward(-0.51 * Height + 0.76 * Lines - 0.36 * (aggregateHeight - Bumpiness) - 0.18 * Bumpiness);
        
        // − 0.51 × Height + 0.76 × Lines − 0.36 × Holes − 0.18 × Bumpiness (5) And the reward was simply the change in this fitness function.
        // holes is calculated as the height of the piece minus the bumpiness, meaning the left over pieces left in the tetris 
    }

    public override void AgentAction() {

        if (FieldMatrix[y, i] = null) { //if you just cleared a line, add a positive reward 
            addreward(1.0f);
            collectObservations();
        }

        if (FourLineBonus = FieldMatrix.GetLength(1)) { //if you cleared four lines and get a bonus, add a reward 
            //four-line bonus 
            addreward(1.0f);
            collectObservations();
        }

        if (IsMovingHorizontal = true) { //if the piece is moving, in order to prevent excessive moving, add negative reward 
            addReward(-0.05f);
            collectObservations();
        }

        if (Input.GetButton("Jump") && Time.time > NextSwap) { // if the player wants to jump the piece, add a negative reward 
            addReward(-0.05f);
            collectObservations();
        }

        if (if (line <= FieldSize.Top) ) { //if the game is over 
            addreward(-0.1f);
            collectObservations();
        }

        if (FieldMatrix[(int)child.position.y, (int)child.position.x] != null) { //if the game is over 
            addreward(-0.1f);
            collectObservations();
        }

        if (State == TetrimoState.Preview) { // if there is another piece in the preview 
            addreward(0.05f);
            collectObservations();

        }

        if ( MatchCamera.Level = 1 + Mathf.FloorToInt(((float)MatchCamera.Scores) / MatchCamera.ScoresForNextLevel)) { //if the game has gone to the next level 
            addreward(1.0f);
            collectObservations();
        }

        if (FieldMatrix[y, x] = null) { //if you just cleared a piece 
            addReward(1.0f);
            collectObservations();
        }



        Vector3 controlSignal = Vector3.zero; //observation pieces 
        controlSignal.x = action[0];
        controlSignal.z = action[1];
        rBody.AddForce(controlSignal * FallingSpeed);
    }
    #endregion

    #region Debugging helper function
    private void KillAndReload() {
        for (int c = FieldSize.Left; c <= FieldSize.Right; c++) {
            for (int r = FieldSize.Bottom; r <= FieldSize.Top; r++) {
                if (FieldMatrix[r, c] != null) {
                    Destroy(FieldMatrix[r, c]);
                }
            }
        }
        Tetrimo[] gameObjects = GetComponents<Tetrimo>();
        foreach (Tetrimo go in gameObjects) {
            if (go.State == TetrimoState.Falling)
                continue;

            Destroy(go.gameObject);
        }

        for (int c = FieldSize.Left; c <= FieldSize.Right; c++) {
            for (int r = FieldSize.Bottom; r <= FieldSize.Top; r++) {
                if (FieldMatrix[r, c] != null) {
                    FieldMatrix[r, c] = (GameObject) Instantiate(TetrimoPartPrefab, new Vector2(c,r), Quaternion.identity);
                    FieldMatrix[r, c].GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
    }


    #endregion
}
