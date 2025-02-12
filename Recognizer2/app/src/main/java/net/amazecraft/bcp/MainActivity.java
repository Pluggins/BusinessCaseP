package net.amazecraft.bcp;

import androidx.appcompat.app.AppCompatActivity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.SurfaceTexture;
import android.hardware.Camera;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.StrictMode;
import android.provider.MediaStore;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.LinearLayout;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.Timer;
import java.util.TimerTask;

public class MainActivity extends AppCompatActivity  {
    private static final String recognizerId = "E5CE0AD5-C0B2-481C-A377-69EB860C237D";
    private static final String recognizerKey = "AE9DD32F-0DC2-4FC4-9BD0-FD8A95673F82";
    private static Camera mCamera;
    private CameraPreview mPreview;
    private static Camera.PictureCallback mPicture;
    private Context myContext;
    private LinearLayout cameraPreview;
    private Button capture;
    private boolean cameraFront = false;
    public static Bitmap bitmap;
    private static boolean stopLoading = false;
    private Timer timer;
    private int taskId = -1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);

        capture = (Button) findViewById(R.id.btnCam);

        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        myContext = this;

        mCamera =  Camera.open();
        mPicture = getPictureCallback();
        cameraPreview = (LinearLayout) findViewById(R.id.cPreview);
        mPreview = new CameraPreview(myContext, mCamera);
        cameraPreview.addView(mPreview);
        SurfaceTexture st = new SurfaceTexture(MODE_PRIVATE);
        try {
            mCamera.setPreviewTexture(st);
            mCamera.startPreview();
            changeDimensionHorizontal();
        } catch (IOException e) {
            e.printStackTrace();
        }

        timer = new Timer();
        TimerTask task = new TimerTask() {
            public void run() {
                if (!stopLoading) {
                    CheckStatus captureStatus = new CheckStatus();
                    captureStatus.execute();
                }
            }
        };
        timer.scheduleAtFixedRate(task, 2000,500);
    }

    public static String getRecognizerId() {
        return recognizerId;
    }

    public static String getRecognizerKey() {
        return recognizerKey;
    }

    private int findFrontFacingCamera() {

        int cameraId = -1;
        // Search for the front facing camera
        int numberOfCameras = Camera.getNumberOfCameras();
        for (int i = 0; i < numberOfCameras; i++) {
            Camera.CameraInfo info = new Camera.CameraInfo();
            Camera.getCameraInfo(i, info);
            if (info.facing == Camera.CameraInfo.CAMERA_FACING_FRONT) {
                cameraId = i;
                cameraFront = true;
                break;
            }
        }
        return cameraId;
    }

    private void changeDimensionVertical() {
        mCamera.setDisplayOrientation(0);
    }

    private void changeDimensionHorizontal() {
        mCamera.setDisplayOrientation(90);
    }

    private int findBackFacingCamera() {
        int cameraId = -1;
        //Search for the back facing camera
        //get the number of cameras
        int numberOfCameras = Camera.getNumberOfCameras();
        //for every camera check
        for (int i = 0; i < numberOfCameras; i++) {
            Camera.CameraInfo info = new Camera.CameraInfo();
            Camera.getCameraInfo(i, info);
            if (info.facing == Camera.CameraInfo.CAMERA_FACING_BACK) {
                cameraId = i;
                cameraFront = false;
                break;

            }

        }
        return cameraId;
    }

    public static void turnOnLoading() {
        MainActivity.stopLoading = false;
    }

    public static void turnOffLoading() {
        MainActivity.stopLoading = true;
    }

    public void onResume() {

        super.onResume();
        if(mCamera == null) {
            mCamera = Camera.open();
            mPreview.refreshCamera(mCamera);
            Log.d("nu", "null");
        }else {
            Log.d("nu","no null");
        }

    }

    @Override
    protected void onPause() {
        super.onPause();
        //when on Pause, release camera in order to be used from other applications
        releaseCamera();
    }

    private void releaseCamera() {
        // stop and release camera
        if (mCamera != null) {
            mCamera.stopPreview();
            mCamera.setPreviewCallback(null);
            mCamera.release();
            mCamera = null;
        }
    }

    private Camera.PictureCallback getPictureCallback() {
        Camera.PictureCallback picture = new Camera.PictureCallback() {
            @Override
            public void onPictureTaken(byte[] data, Camera camera) {
                    mCamera.startPreview();

                bitmap = BitmapFactory.decodeByteArray(data, 0, data.length);
                if (taskId == 1) {
                    String s = RequestService.RequestImage("https://bcp.amazecraft.net/Api/Recognizer/UploadImage", bitmap);
                    try {
                        JSONObject tmpObj = new JSONObject(s);
                        StudentService.insertUrl(tmpObj.getString("url"));
                        StudentService.execute();
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                } else if (taskId == 2) {
                    String s = RequestService.RequestImage("https://bcp.amazecraft.net/Api/Recognizer/UploadImage", bitmap);
                    try {
                        JSONObject tmpObj = new JSONObject(s);
                        ClassService.insertUrl(tmpObj.getString("url"));
                        ClassService.execute();
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }

            }
        };
        return picture;
    }

    public static void takePicture() {
        mCamera.takePicture(null, null, mPicture);
    }

    private class CheckStatus extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String[] params) {
            String result = null;
            JSONObject tmpObj = new JSONObject();
            try {
                tmpObj.put("recognizerId", recognizerId);
                tmpObj.put("recognizerKey", recognizerKey);
                result = RequestService.RequestJSON("https://bcp.amazecraft.net/Api/Recognizer/FetchInstruction", tmpObj);
            } catch (JSONException e) {
                e.printStackTrace();
            }
            return result;
        }

        @Override
        protected void onPostExecute(String message) {
            try {
                JSONObject result = new JSONObject(message);
                String s = result.getString("result");
                if (s.equals("OK")) {

                    //Get phone orientation
                    Camera.CameraInfo info = new Camera.CameraInfo();
                    Camera.getCameraInfo(Camera.CameraInfo.CAMERA_FACING_BACK, info);
                    int rotation = getWindowManager().getDefaultDisplay().getRotation();
                    int degrees = 0;
                    switch (rotation) {
                        case Surface.ROTATION_0: degrees = 0; break; //Natural orientation
                        case Surface.ROTATION_90: degrees = 90; break; //Landscape left
                        case Surface.ROTATION_180: degrees = 180; break;//Upside down
                        case Surface.ROTATION_270: degrees = 270; break;//Landscape right
                    }
                    int rotate = (info.orientation - degrees + 360) % 360;

                    Camera.Parameters params = mCamera.getParameters();
                    params.setRotation(rotate);
                    mCamera.setParameters(params);

                    stopLoading = true;
                    String c = result.getString("command");
                    if (c.equals("REGISTER_NEW_FACE")) {
                        taskId = 1;
                        StudentService.clear();
                        StudentService.setTaskId(result.getString("taskId"));
                        StudentService.setStudentId(result.getString("primaryValue"));
                        StudentService.setNumCapture(result.getInt("secondaryValue"));
                        StudentService.execute();
                    } else if (c.equals("CAPTURE_CLASS_IMAGE")) {
                        taskId = 2;
                        ClassService.clear();
                        ClassService.setTaskId(result.getString("taskId"));
                        ClassService.setClassId(result.getString("primaryValue"));
                        ClassService.setNumCapture(result.getInt("secondaryValue"));
                        ClassService.execute();
                    }
                }
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
    }
}
