package net.amazecraft.bcp;

import android.os.AsyncTask;
import android.os.Handler;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

public class StudentService {
    private static String studentId;
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
            LinkUser lu = new LinkUser();
            lu.execute();
        }
    }

    public static void clear() {
        StudentService.studentId = null;
        StudentService.numCapture = -1;
        StudentService.urls = new ArrayList<String>();
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

    public static String getStudentId() {
        return studentId;
    }

    public static void setStudentId(String studentId) {
        StudentService.studentId = studentId;
    }

    public static int getNumCapture() {
        return numCapture;
    }

    public static void setNumCapture(int numCapture) {
        StudentService.numCapture = numCapture;
    }

    public static boolean isDone() {
        return done;
    }

    public static void setDone(boolean done) {
        StudentService.done = done;
    }

    public static void insertUrl(String url) {
        StudentService.urls.add(url);
    }

    public static List<String> retrieveUrls() {
        return StudentService.urls;
    }

    public static String getTaskId() {
        return taskId;
    }

    public static void setTaskId(String taskId) {
        StudentService.taskId = taskId;
    }

    private static class LinkUser extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String[] params) {
            for (String item : StudentService.retrieveUrls()) {
                JSONObject tmpObj = new JSONObject();
                try {
                    tmpObj.put("recognizerId", MainActivity.getRecognizerId());
                    tmpObj.put("recognizerKey", MainActivity.getRecognizerKey());
                    tmpObj.put("studentId", StudentService.getStudentId());
                    tmpObj.put("imageUrl", item);
                    RequestService.RequestJSON("https://bcp.amazecraft.net/Api/Student/AddPhoto", tmpObj);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            JSONObject tmpObj = new JSONObject();
            try {
                tmpObj.put("recognizerId", MainActivity.getRecognizerId());
                tmpObj.put("recognizerKey", MainActivity.getRecognizerKey());
                tmpObj.put("recognizerTaskId", StudentService.getTaskId());
                RequestService.RequestJSON("https://bcp.amazecraft.net/Api/Recognizer/MarkTaskComplete", tmpObj);
                MainActivity.turnOnLoading();
            } catch (JSONException e) {
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onPostExecute(String message) {
            StudentService.clear();
            MainActivity.turnOffLoading();
        }
    }
}

