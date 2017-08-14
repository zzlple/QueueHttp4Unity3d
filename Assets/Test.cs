using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {


		//init multi queue=8
		HttpHelper.init (8);
		string[] str = new string[]{ "https://raw.githubusercontent.com/zzlple/zzlple.github.io/master/music/%E7%8E%8B%E6%9D%B0%20-%20%E4%B8%8D%E6%B5%AA%E6%BC%AB%E7%BD%AA%E5%90%8D.mp3"
			,"https://raw.githubusercontent.com/zzlple/zzlple.github.io/master/music/%E7%8E%8B%E6%9D%B0%20-%20%E5%A5%B9%E7%9A%84%E8%83%8C%E5%BD%B1.mp3",
			"https://raw.githubusercontent.com/zzlple/zzlple.github.io/master/music/%E8%AE%B8%E5%B7%8D%20-%20%E5%96%9D%E8%8C%B6%E5%8E%BB.mp3"
		};


		//in queue
		for (int i = 0; i < str.Length; i++) {
		

			HttpHelper.Async (str[i], HttpHelper.HttpMethod.GET, null, delegate(bool result, WWW www) {
				if(result){
					Debug.Log("the res download is success");
				}else{
					Debug.Log("the res download is failed");
				}

			},delegate(float downloadProgress, float uploadProgress) {

	


				Debug.Log("the res download progress is"+downloadProgress);

			});
		}

	
	}
}
