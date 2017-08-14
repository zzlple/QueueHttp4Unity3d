

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;





public delegate void ResultCallback (bool result, WWW www);
public delegate void DiskFileCallback (bool result, string file);
public delegate void OnProgress (float downloadProgress, float uploadProgress);




public class HttpHelper : MonoBehaviour
{
	private const string httphelper = "[Http]-";
	/// <summary>
	/// The queue count.初始化队列数量
	/// </summary>
	public static int queueCount = 1;
	/// <summary>
	/// The instance.队列对象实例
	/// </summary>
	private static HttpHelper[] _instance;
	/// <summary>
	/// The sand box path.
	/// </summary>
	public static string _SandBoxPath;
	/// <summary>
	/// The download queue.下载队列
	/// </summary>
	public  Queue<Job> _downloadQueue;
	/// <summary>
	/// The job is processing.队列是否解析
	/// </summary>
	public  bool _jobIsProcessing;
	/// <summary>
	/// The http method.请求方法
	/// </summary>
	private HttpMethod httpMethod;
	/// <summary>
	/// The www.www实例
	/// </summary>
	private  WWW www;
	/// <summary>
	/// The is active.是否激活
	/// </summary>
	private  bool _IsActive;



	/// <summary>
	/// Init the specified queueCount.
	/// </summary>
	/// <param name="queueCount">Queue count.</param>
	public static void init (int queueCount)
	{

		HttpHelper.queueCount = queueCount;
		if (queueCount <= 0) {
			HttpHelper.queueCount = 1;
		}
		if (queueCount >= 8) {
			HttpHelper.queueCount = 8;
		}

	}



	/// <summary>
	///  Downloads a file Async and sends it to the callback.
	///  Invokes the failscript if the download failed
	/// </summary>
	public static void Async (string url, HttpMethod httpMethod, WWWForm wwwFrom, ResultCallback callback, OnProgress progress)
	{

		Job job = new Job (url, httpMethod, wwwFrom, callback, progress, 0);




		int queueIndex =	UnityEngine.Random.Range (0, _singleton.Length - 1);

		_singleton [queueIndex].Enqueue (job);
	}


	public static void Async (string url, HttpMethod httpMethod, WWWForm wwwFrom, ResultCallback callback, OnProgress progress, int downloadRetries)
	{


		Job job = new Job (url, httpMethod, wwwFrom, callback, progress, downloadRetries);
		int queueIndex =	UnityEngine.Random.Range (0, _singleton.Length - 1);
		_singleton [queueIndex].Enqueue (job);
	}


	public static void AsyncCacheOrDownload (string url, ResultCallback callback, OnProgress progress, int version = 0)
	{


		Job job = new Job (url, HttpMethod.CACHEORDOWNLOAD, callback, progress, version, 0);
		int queueIndex =	UnityEngine.Random.Range (0, _singleton.Length - 1);
		_singleton [queueIndex].Enqueue (job);

	}

	public static void AsyncFileToDisk (string url, string floder, DiskFileCallback diskfilecallback, OnProgress progress, bool reloadFromServer = false)
	{

		Job job = new Job (url, floder, HttpMethod.FILETODISK, diskfilecallback, progress, 0, reloadFromServer);
		int queueIndex =	UnityEngine.Random.Range (0, _singleton.Length - 1);
		_singleton [queueIndex].Enqueue (job);





	}






	public enum HttpMethod
	{
		GET,
		POST,
		CACHEORDOWNLOAD,
		FILETODISK
	}



	/////////////////////////////////////////////////
	//////-------------Public API--------------//////
	/////////////////////////////////////////////////

	/// <summary>
	/// Clears the download queue.
	/// </summary>
	public static void ClearQueue ()
	{




	


		if (null != _singleton && _singleton.Length > 0) {

			for (int i = 0; i < _singleton.Length; i++) {

				if (null != _singleton [i].www) {
				
					ReleaseResource (_singleton [i].www, _singleton [i].httpMethod);
				}
					

				if (null != _singleton [i]._downloadQueue) {
					_singleton [i]._downloadQueue.Clear ();
				}
				
				_singleton [i].StopAllCoroutines ();

				_singleton [i]._jobIsProcessing = false;
				
			}

	
		}
	

	

	}






	public static string getLastName (string longString)
	{


		return longString.Substring (longString.LastIndexOf ("/") + 1);
	}

	public static string CreateFile (string path, string name, byte[] bin)
	{


		FileInfo fileInfo = new FileInfo (path + "/" + name);

		if (fileInfo.Exists) {
			fileInfo.Delete ();

		}

		if (!System.IO.Directory.Exists (path)) {

			System.IO.Directory.CreateDirectory (path);

		}



		File.WriteAllBytes (path + "/" + name, bin);


		return path + "/" + name;

	}



	/// <summary>
	/// Gets a value indicating whether this instance is active.
	/// </summary>
	/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
	public  bool IsActive {
		get {
			return _IsActive;
		}
	}



	/////////////////////////////////////////////////
	//////-------------Queue Object--------------//////
	/////////////////////////////////////////////////
	public sealed class Job
	{
		public string url { get; set; }

		public ResultCallback callback { get; set; }

		public DiskFileCallback diskfilecallback{ get; set; }

		public OnProgress progress{ get; set; }

		public int downloadRetries { get; set; }

		public int version{ get; set; }

		public string floder{ get; set; }

		public bool reloadFromServer{ get; set; }

		public HttpMethod httpMethod{ get; set; }

		public WWWForm wwwFrom{ get; set; }

		public Job (string url, HttpMethod httpMethod, WWWForm wwwFrom, ResultCallback callback, OnProgress progress, int downloadRetries)
		{
			this.url = url;
			this.callback = callback;
			this.progress = progress;
			this.httpMethod = httpMethod;
			this.wwwFrom = wwwFrom;
			if (downloadRetries != default(int)) {
				this.downloadRetries = downloadRetries;
			} else {
				this.downloadRetries = 0;
			}
		}

		public Job (string url, HttpMethod httpMethod, ResultCallback callback, OnProgress progress, int version, int downloadRetries)
		{
			this.url = url;
			this.version = version;
			this.callback = callback;
			this.progress = progress;
			this.httpMethod = httpMethod;
			if (downloadRetries != default(int)) {
				this.downloadRetries = downloadRetries;
			} else {
				this.downloadRetries = 0;
			}
		}

		public Job (string url, string floder, HttpMethod httpMethod, DiskFileCallback diskfilecallback, OnProgress progress, int downloadRetries, bool reloadFromServer)
		{
			this.url = url;
			this.floder = floder;
			this.diskfilecallback = diskfilecallback;
			this.reloadFromServer = reloadFromServer;
			this.progress = progress;
			this.httpMethod = httpMethod;
			if (downloadRetries != default(int)) {
				this.downloadRetries = downloadRetries;
			} else {
				this.downloadRetries = 0;
			}
		}
	}

	/////////////////////////////////////////////////
	//////---------Instance Members------------//////
	/////////////////////////////////////////////////

	#region Singleton



	private static HttpHelper[] _singleton {
		get {
			_instance = GameObject.FindObjectsOfType<HttpHelper> ();
			if (_instance == null || _instance.Length < queueCount || _instance.Length == 0) {

			
					
				GameObject temp = new GameObject (httphelper + System.DateTime.Now.ToString ("yyyy_MM_dd_hh_mm_ss_fff"));
				temp.AddComponent (typeof(HttpHelper));


				_instance = GameObject.FindObjectsOfType<HttpHelper> ();
			}
			Debug.Log (_instance.Length + "-----===");
			return _instance;
		}
	}

	#endregion



	void Awake ()
	{
		_IsActive = false;
		_jobIsProcessing = false;
		_downloadQueue = new Queue<Job> ();
		_SandBoxPath = Application.persistentDataPath;
	
	}




	void LateUpdate ()
	{



		if (null != _downloadQueue && _downloadQueue.Count == 0) {
			_IsActive = false;


			if (!_jobIsProcessing && null != www) {

				Debug.Log ("destory-----");

				Destroy (gameObject);
			
			}

			return;
		}

		_IsActive = true;

		if (!_jobIsProcessing
		    && Internet.isConnected ()) {
			StartCoroutine (ProcessJob ());
		}
	}




	private static  void ReleaseResource (WWW www, HttpMethod method)
	{




		try {

			if (www.isDone == false) {

				www.Dispose ();
			}
			if (null != www.error) {

				return;
			}
				
			if (null != www && null != www.assetBundle) {
				www.assetBundle.Unload (false);
			}
			if (method != HttpMethod.CACHEORDOWNLOAD) {
				
				if (null != www && null != www.texture) {
					Destroy (www.texture);
				}
				
			}
				
				
				
			www.Dispose ();
			www = null;
			Resources.UnloadUnusedAssets ();
			System.GC.Collect ();
		} catch (Exception ex) {
				
		} 
	}


	private  IEnumerator ProcessJob ()
	{
		_jobIsProcessing = true;

		Job job = _downloadQueue.Dequeue () as Job;

		yield return null;

		if (job != null) {
			for (int i = 0; i <= job.downloadRetries; i++) {
				//initiate working variables
				string path = "";
				string name = "";
				WWW www = null;
				switch (job.httpMethod) {

				case HttpMethod.GET:
					www = new WWW (job.url);
					break;

				case HttpMethod.POST:
					www = new WWW (job.url, job.wwwFrom);
					break;

				case HttpMethod.CACHEORDOWNLOAD:

					www = WWW.LoadFromCacheOrDownload (job.url, job.version);
					break;


				case HttpMethod.FILETODISK:
					path = _SandBoxPath + "/" + job.floder;
					name = getLastName (job.url);
					if (job.reloadFromServer) {

						www = new WWW (job.url);
					
					} else {

						if (File.Exists (path + "/" + name) && new FileInfo (path + "/" + name).Length > 0) {
						
							if (job.diskfilecallback != null) {
								job.diskfilecallback (true, path + "/" + name);
								continue;
							}
						
						} else {
						
						
							www = new WWW (job.url);
						}

					}


					break;


				}

				Debug.Log (gameObject.name + "--" + "request url:" + www.url);


				www.threadPriority = ThreadPriority.Low;
				this.www = www;
				this.httpMethod = job.httpMethod;

				while (!this.www.isDone) {

					yield return new WaitForEndOfFrame ();
					if (null != job.progress) {
					
						job.progress (this.www.progress, this.www.uploadProgress);

					


					
					}

				}
				if (this.www.isDone) {
				
					job.progress (1, 1);
				}


				yield return www;

				if (www.error != null) {
					Debug.Log ("Download Error: " + www.url + " : " + www.error);
					if (job.callback != null) {
						job.callback (false, null);
						ReleaseResource (www, job.httpMethod);
					}
					if (job.diskfilecallback != null) {
						job.diskfilecallback (false, null);
						ReleaseResource (www, job.httpMethod);
					}
					continue;
				}
				if (job.callback != null) {
					job.callback (true, www);
					ReleaseResource (www, job.httpMethod);
				}


				if (job.diskfilecallback != null) {

					job.diskfilecallback (true, CreateFile (path, name, www.bytes));
					ReleaseResource (www, job.httpMethod);
				
				}





				_jobIsProcessing = false;

				break;
			}
			_jobIsProcessing = false;
		}


	}

	private void Enqueue (Job job)
	{
		_downloadQueue.Enqueue (job);
	}

}

public static class Internet
{

	public static bool isConnected ()
	{
		return (Network.player.ipAddress.ToString () == "127.0.0.1") ? false : true;
	}
}
