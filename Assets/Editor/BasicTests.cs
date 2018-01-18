using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class BasicTests {

	[Test]
	public void BasicTestsSimplePasses() {
        // Use the Assert class to test conditions.
        GameObject.Find("Player1");
        //Assert.AreEqual(.1, .5);
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator BasicTestsWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
