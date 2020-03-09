package edmt.dev.facedetectmlkit.Model;

public class Student {
    private String Name;
    private int UserType;

    public Student() {
    }

    public Student(String name, int userType) {
        Name = name;
        UserType = userType;
    }

    public int getUserType() {
        return UserType;
    }

    public void setUserType(int userType) {
        UserType = userType;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }
}
