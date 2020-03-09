package edmt.dev.facedetectmlkit;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class Signup extends AppCompatActivity {
    EditText edtEmail, edtPass1, edtPass2;
    TextView txtLogin;
    Button btnSignup;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_signup);

        edtEmail = (EditText)findViewById(R.id.edtEmail);
        edtPass1 = (EditText)findViewById(R.id.edtPassword);
        edtPass2 = (EditText)findViewById(R.id.edtConfirmPassword);
        txtLogin = (TextView)findViewById(R.id.txtLogin);
        btnSignup = (Button)findViewById(R.id.btnSignup);

        btnSignup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //Signup
            }
        });

        txtLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent login = new Intent(Signup.this, Login.class);
                startActivity(login);
            }
        });
    }

    private void signUser() {}
}
