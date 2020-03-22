package net.amazecraft.bcp;

import android.graphics.Bitmap;
import android.os.NetworkOnMainThreadException;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.UUID;

public class RequestService {
    private static URL url = null;
    private static String response = null;

    public static String RequestJSON(String siteUrl, JSONObject jsonRequest) {
        response = null;

        try {
            url = new URL(siteUrl);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }

        try {
            HttpURLConnection con = (HttpURLConnection) url.openConnection();
            con.setRequestMethod("POST");
            con.setRequestProperty("Content-Type", "application/json");
            con.setRequestProperty("Accept", "application/json");
            con.setDoInput(true);

            OutputStreamWriter wr = new OutputStreamWriter(con.getOutputStream());
            wr.write(jsonRequest.toString());
            wr.flush();

            try(BufferedReader br = new BufferedReader(new InputStreamReader(con.getInputStream(), "utf-8"))) {
                StringBuilder sb = new StringBuilder();
                String responseLine = null;
                while ((responseLine = br.readLine()) != null) {
                    sb.append(responseLine.trim());
                }
                response = sb.toString();
            }
        } catch (IOException e) {
            e.printStackTrace();
        } catch (NetworkOnMainThreadException e) {
            e.printStackTrace();
        }

        return response;
    }

    public static String RequestImage(String siteUrl, Bitmap bitmap) {
        response = null;

        try {
            url = new URL(siteUrl);
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }

        try {
            HttpURLConnection con = (HttpURLConnection) url.openConnection();
            String attachmentName = "test";
            String attachmentFileName = "test.png";
            String crlf = "\r\n";
            String twoHyphens = "--";
            String boundary =  "*****";
            con.setRequestMethod("POST");
            con.setRequestProperty("Connection", "Keep-Alive");
            con.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + boundary);
            con.setDoInput(true);

            DataOutputStream dos = new DataOutputStream(con.getOutputStream());
            dos.writeBytes(twoHyphens + boundary + crlf);
            dos.writeBytes("Content-Disposition: form-data; name=\"" +
                    attachmentName + "\";filename=\"" +
                    attachmentFileName + "\"" + crlf);
            dos.writeBytes(crlf);

            ByteArrayOutputStream imageStream =new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.PNG, 100, imageStream);
            byte[] imageInByte=imageStream.toByteArray();

            dos.write(imageInByte);

            dos.writeBytes(crlf);
            dos.writeBytes(twoHyphens + boundary +
                    twoHyphens + crlf);

            dos.flush();
            dos.close();

            try(BufferedReader br = new BufferedReader(new InputStreamReader(con.getInputStream(), "utf-8"))) {
                StringBuilder sb = new StringBuilder();
                String responseLine = null;
                while ((responseLine = br.readLine()) != null) {
                    sb.append(responseLine.trim());
                }
                response = sb.toString();
            }
        } catch (IOException e) {
            e.printStackTrace();
        } catch (NetworkOnMainThreadException e) {
            e.printStackTrace();
        }

        return response;
    }
}
