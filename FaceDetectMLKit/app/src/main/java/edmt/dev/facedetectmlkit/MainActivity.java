package edmt.dev.facedetectmlkit;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.FileProvider;

import android.app.AlertDialog;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Rect;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Base64;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.firebase.ml.vision.FirebaseVision;
import com.google.firebase.ml.vision.common.FirebaseVisionImage;
import com.google.firebase.ml.vision.face.FirebaseVisionFace;
import com.google.firebase.ml.vision.face.FirebaseVisionFaceDetector;
import com.google.firebase.ml.vision.face.FirebaseVisionFaceDetectorOptions;
import com.wonderkiln.camerakit.CameraKitError;
import com.wonderkiln.camerakit.CameraKitEvent;
import com.wonderkiln.camerakit.CameraKitEventListener;
import com.wonderkiln.camerakit.CameraKitImage;
import com.wonderkiln.camerakit.CameraKitVideo;
import com.wonderkiln.camerakit.CameraView;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import dmax.dialog.SpotsDialog;
import edmt.dev.facedetectmlkit.Helper.GraphicOverlay;
import edmt.dev.facedetectmlkit.Helper.ImageClass;
import edmt.dev.facedetectmlkit.Helper.NetworkClient;
import edmt.dev.facedetectmlkit.Helper.RectOverlay;
import edmt.dev.facedetectmlkit.Helper.UploadImage;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;

public class MainActivity extends AppCompatActivity {

    CameraView cameraView;
    GraphicOverlay graphicOverlay;
    Button btnDetect;

    AlertDialog waitingDialog;

    String filePaths = "";

    @Override
    protected void onResume(){
        super.onResume();
        cameraView.start();
    }

    @Override
    protected void onPause(){
        super.onPause();
        cameraView.stop();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        cameraView = (CameraView)findViewById(R.id.camera_view);
        graphicOverlay = (GraphicOverlay)findViewById(R.id.grayscale_overlay);
        btnDetect = (Button)findViewById(R.id.btn_detect);
        waitingDialog = new SpotsDialog.Builder().setContext(this)
                .setMessage("Please wait")
                .setCancelable(false)
                .build();


        btnDetect.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                cameraView.start();
                cameraView.captureImage();
                graphicOverlay.clear();
            }
        });

        cameraView.addCameraKitListener(new CameraKitEventListener() {
            @Override
            public void onEvent(CameraKitEvent cameraKitEvent) {

            }

            @Override
            public void onError(CameraKitError cameraKitError) {

            }

            @Override
            public void onImage(CameraKitImage cameraKitImage) {


             waitingDialog.show();

             Bitmap bitmap = cameraKitImage.getBitmap();
             bitmap = Bitmap.createScaledBitmap(bitmap,cameraView.getWidth(),cameraView.getHeight(),false);
             cameraView.stop();

             runFaceDetector(bitmap);

             uploadImg(bitmap);

            }

            @Override
            public void onVideo(CameraKitVideo cameraKitVideo) {

            }
        });

    }

    private void runFaceDetector(Bitmap bitmap) {
        FirebaseVisionImage image =  FirebaseVisionImage.fromBitmap(bitmap);


        FirebaseVisionFaceDetectorOptions options = new FirebaseVisionFaceDetectorOptions.Builder()
                .build();

        FirebaseVisionFaceDetector detector = FirebaseVision.getInstance()
                .getVisionFaceDetector(options);

        detector.detectInImage(image)
                .addOnSuccessListener(new OnSuccessListener<List<FirebaseVisionFace>>() {
                    @Override
                    public void onSuccess(List<FirebaseVisionFace> firebaseVisionFaces) {
                        processFaceResult(firebaseVisionFaces);
                    }
                })
                .addOnFailureListener(new OnFailureListener() {
                    @Override
                    public void onFailure(@NonNull Exception e) {
                        Toast.makeText(MainActivity.this,e.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
    }

    private void processFaceResult(List<FirebaseVisionFace> firebaseVisionFaces) {
        int count = 0;
        for(FirebaseVisionFace face :firebaseVisionFaces)
        {
            Rect bounds = face.getBoundingBox();

            RectOverlay rect = new RectOverlay(graphicOverlay,bounds);
            graphicOverlay.add(rect);

            count++;
        }
        waitingDialog.dismiss();
        Toast.makeText(this, String.format("Detected %d faces in image",count), Toast.LENGTH_SHORT).show();
    }

    private void uploadImg(Bitmap bm) {
        String img = imageString(bm);
        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
        String imageFileName = "JPEG_" + timeStamp + "_";

        UploadImage api = NetworkClient.getRetrofit().create(UploadImage.class);
        Call<ImageClass> call = api.uploadImage(imageFileName, img);
        call.enqueue(new Callback<ImageClass>() {
            @Override
            public void onResponse(Call<ImageClass> call, Response<ImageClass> response) {

            }

            @Override
            public void onFailure(Call<ImageClass> call, Throwable t) {

            }
        });
    }

    private String imageString(Bitmap bm) {
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        bm.compress(Bitmap.CompressFormat.JPEG,100,outputStream);
        byte[] imgByte = outputStream.toByteArray();

        return Base64.encodeToString(imgByte, Base64.DEFAULT);
    }
}
