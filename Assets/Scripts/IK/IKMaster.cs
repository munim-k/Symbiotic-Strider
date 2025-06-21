using UnityEngine;

public class IKMaster : MonoBehaviour {
    [SerializeField] private IKBoneTarget[] ikTargetsLeft;
    [SerializeField] private IKBoneTarget[] ikTargetsRight;
    private bool turn = false;

    private void Start() {
        //Let left targets move first
        foreach (IKBoneTarget target in ikTargetsLeft) {
            target.SetTurn(true);
        }
        foreach (IKBoneTarget target in ikTargetsRight) {
            target.SetTurn(false);
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        bool result = false;
        if (!turn) {
            //Left targets were moving
            foreach (IKBoneTarget target in ikTargetsLeft) {
                result = result || target.GetTurn();
            }
            //If all left targets are done moving then switch to right targets
            if (!result) {
                turn = !turn;
                foreach (IKBoneTarget target in ikTargetsRight) {
                    target.SetTurn(true);
                }
                foreach (IKBoneTarget target in ikTargetsLeft) {
                    target.SetTurn(false);
                }
            }
        }
        else {
            //Right targets were moving
            foreach (IKBoneTarget target in ikTargetsRight) {
                result = result || target.GetTurn();
            }
            if (!result) {

                turn = !turn;
                foreach (IKBoneTarget target in ikTargetsLeft) {
                    target.SetTurn(true);
                }
                foreach (IKBoneTarget target in ikTargetsRight) {
                    target.SetTurn(false);
                }
            }
        }
    }
}
