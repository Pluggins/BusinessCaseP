package net.amazecraft.bcp;

import android.os.AsyncTask;
import android.os.Handler;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class ClassService {
    private static String classId;
    private static int numCapture;
    private static boolean done = false;
    private static List<String> urls;
    private static String taskId;

    public static void execute() {
        if (nextCapture()) {
            final Handler handler = new Handler();
            handler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    // Do something after 5s = 5000ms
                    MainActivity.takePicture();
                }
            }, 3000);
        } else {
            ClassService.LinkClass lc = new ClassService.LinkClass();
            lc.execute();
        }
    }

    public static void clear() {
        ClassService.classId = null;
        ClassService.numCapture = -1;
        ClassService.urls = new ArrayList<String>();
    }

    public static boolean nextCapture() {
        if (numCapture <= 0) {
            numCapture = -1;
            return false;
        } else {
            numCapture--;
            return true;
        }
    }

    public static String getClassId() {
        return classId;
    }

    public static void setClassId(String classId) {
        ClassService.classId = classId;
    }

    public static int getNumCapture() {
        return numCapture;
    }

    public static void setNumCapture(int numCapture) {
        ClassService.numCapture = numCapture;
    }

    public static boolean isDone() {
        return done;
    }

    public static void setDone(boolean done) {
        ClassService.done = done;
    }

    public static String getTaskId() {
        return taskId;
    }

    public static void setTaskId(String taskId) {
        ClassService.taskId = taskId;
    }

    public static void insertUrl(String url) {
        ClassService.urls.add(url);
    }

    public static List<String> retrieveUrls() {
        return ClassService.urls;
    }

    private static class LinkClass extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String[] params) {
            for (String item : ClassService.retrieveUrls()) {
                JSONObject tmpObj = new JSONObject();
                try {
                    tmpObj.put("recognizerId", MainActivity.getRecognizerId());
                    tmpObj.put("recognizerKey", MainActivity.getRecognizerKey());
                    tmpObj.put("classId", ClassService.getClassId());
                    tmpObj.put("imageUrl", item);
                    RequestService.RequestJSON("https://bcp.amazecraft.net/Api/Class/AddPhoto", tmpObj);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            JSONObject tmpObj = new JSONObject();
            try {
                tmpObj.put("recognizerId", MainActivity.getRecognizerId());
                tmpObj.put("recognizerKey", MainActivity.getRecognizerKey());
                tmpObj.put("recognizerTaskId", ClassService.getTaskId());
                RequestService.RequestJSON("https://bcp.amazecraft.net/Api/Recognizer/MarkTaskComplete", tmpObj);
                MainActivity.turnOnLoading();
            } catch (JSONException e) {
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onPostExecute(String message) {
            ClassService.clear();
            MainActivity.turnOffLoading();
        }
    }
}
