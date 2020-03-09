package edmt.dev.facedetectmlkit.Helper;

import okhttp3.MultipartBody;
import okhttp3.RequestBody;
import retrofit2.Call;
import retrofit2.http.Field;
import retrofit2.http.Multipart;
import retrofit2.http.POST;
import retrofit2.http.Part;

public interface UploadImage {
    @Multipart
    @POST("/Api/Recognizer/UploadImage")
    Call<ImageClass> uploadImage(@Field("title") String title, @Field("image") String image);
//    Call<RequestBody> uploadImage(@Part MultipartBody.Part part, @Part("name")RequestBody requestBody);
}
