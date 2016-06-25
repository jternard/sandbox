package jeremyternard.dnatranscript;

import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;
import android.annotation.TargetApi;
import android.content.pm.PackageManager;
import android.support.annotation.NonNull;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.app.LoaderManager.LoaderCallbacks;

import android.content.CursorLoader;
import android.content.Loader;
import android.database.Cursor;
import android.net.Uri;
import android.os.AsyncTask;

import android.os.Build;
import android.os.Bundle;
import android.provider.ContactsContract;
import android.text.TextUtils;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.inputmethod.EditorInfo;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;

import com.google.android.gms.appindexing.Action;
import com.google.android.gms.appindexing.AppIndex;
import com.google.android.gms.common.api.GoogleApiClient;

import org.w3c.dom.Text;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import static android.Manifest.permission.READ_CONTACTS;


public class MainPage extends AppCompatActivity implements OnClickListener {

    private EditText mInputNameView;
    private EditText mInputDNAView;
    private TextView mOutputNameView;
    private ImageView mImageResult;

    public ImageView getImageResult() {
        mImageResult = (ImageView) findViewById(R.id.image_result);
        return mImageResult;
    }

    /**
     * ATTENTION: This was auto-generated to implement the App Indexing API.
     * See https://g.co/AppIndexing/AndroidStudio for more information.
     */
    private GoogleApiClient client;

    public EditText getInputNameView() {
        mInputNameView = (EditText) findViewById(R.id.input_name);
        return mInputNameView;
    }

    private TextView mOutputDNA;

    public TextView getOutputDNA() {
        mOutputDNA = (TextView) findViewById(R.id.output_dna);
        return mOutputDNA;
    }

    public EditText getInputDNAView() {
        mInputDNAView = (EditText) findViewById(R.id.input_dna);
        return mInputDNAView;
    }

    public TextView getOutputNameView() {
        mOutputNameView = (TextView) findViewById(R.id.output_name);
        return mOutputNameView;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_page);
        mInputNameView = getInputNameView();
        mOutputDNA = getOutputDNA();
        mInputDNAView = getInputDNAView();
        mOutputNameView = getOutputNameView();
        mImageResult = getImageResult();

        mInputNameView.setOnClickListener(this);
        mInputDNAView.setOnClickListener(this);


        // ATTENTION: This was auto-generated to implement the App Indexing API.
        // See https://g.co/AppIndexing/AndroidStudio for more information.
        client = new GoogleApiClient.Builder(this).addApi(AppIndex.API).build();
    }

    public String textToDNA(String input) {
        Map<String, String> dictionary = new HashMap<String, String>();
        dictionary.put("A", "AAA");
        dictionary.put("B", "AAC");
        dictionary.put("C", "AAG");
        dictionary.put("D", "AAT");
        dictionary.put("E", "ACA");
        dictionary.put("F", "ACC");
        dictionary.put("G", "ACG");
        dictionary.put("H", "ACT");
        dictionary.put("I", "AGA");
        dictionary.put("J", "AGC");
        dictionary.put("K", "AGG");
        dictionary.put("L", "AGT");
        dictionary.put("M", "ATA");
        dictionary.put("N", "ATC");
        dictionary.put("O", "ATG");
        dictionary.put("P", "ATT");
        dictionary.put("Q", "CAA");
        dictionary.put("R", "CAC");
        dictionary.put("S", "CAG");
        dictionary.put("T", "CAT");
        dictionary.put("U", "CCA");
        dictionary.put("V", "CCC");
        dictionary.put("W", "CCG");
        dictionary.put("X", "CCT");
        dictionary.put("Y", "CGA");
        dictionary.put("Z", "CGC");
        dictionary.put("a", "CGG");
        dictionary.put("b", "CGT");
        dictionary.put("c", "CTA");
        dictionary.put("d", "CTC");
        dictionary.put("e", "CTG");
        dictionary.put("f", "CTT");
        dictionary.put("g", "GAA");
        dictionary.put("h", "GAC");
        dictionary.put("i", "GAG");
        dictionary.put("j", "GAT");
        dictionary.put("k", "GCA");
        dictionary.put("l", "GCC");
        dictionary.put("m", "GCG");
        dictionary.put("n", "GCT");
        dictionary.put("o", "GGA");
        dictionary.put("p", "GGC");
        dictionary.put("q", "GGG");
        dictionary.put("r", "GGT");
        dictionary.put("s", "GTA");
        dictionary.put("t", "GTC");
        dictionary.put("u", "GTG");
        dictionary.put("v", "GTT");
        dictionary.put("w", "TAA");
        dictionary.put("x", "TAC");
        dictionary.put("y", "TAG");
        dictionary.put("z", "TAT");
        dictionary.put(" ", "TCA");
        dictionary.put("STOP", "TCC");
        dictionary.put("STOP", "TCG");
        dictionary.put("STOP", "TCT");
        dictionary.put("STOP", "TGA");
        dictionary.put("STOP", "TGC");
        dictionary.put("STOP", "TGG");
        dictionary.put("STOP", "TGT");
        dictionary.put("STOP", "TTA");
        dictionary.put("STOP", "TTC");
        dictionary.put("STOP", "TTG");
        dictionary.put("STOP", "TTT");

        String output = "";
        for (Character letter : input.toCharArray()) {
            output = output + dictionary.get(letter.toString());
        }
        return output;
    }

    public String DNAtoText(String input) {
        Map<String, String> dictionary = new HashMap<String, String>();
        dictionary.put("AAA", "A");
        dictionary.put("AAC", "B");
        dictionary.put("AAG", "C");
        dictionary.put("AAT", "D");
        dictionary.put("ACA", "E");
        dictionary.put("ACC", "F");
        dictionary.put("ACG", "G");
        dictionary.put("ACT", "H");
        dictionary.put("AGA", "I");
        dictionary.put("AGC", "J");
        dictionary.put("AGG", "K");
        dictionary.put("AGT", "L");
        dictionary.put("ATA", "M");
        dictionary.put("ATC", "N");
        dictionary.put("ATG", "O");
        dictionary.put("ATT", "P");
        dictionary.put("CAA", "Q");
        dictionary.put("CAC", "R");
        dictionary.put("CAG", "S");
        dictionary.put("CAT", "T");
        dictionary.put("CCA", "U");
        dictionary.put("CCC", "V");
        dictionary.put("CCG", "W");
        dictionary.put("CCT", "X");
        dictionary.put("CGA", "Y");
        dictionary.put("CGC", "Z");
        dictionary.put("CGG", "a");
        dictionary.put("CGT", "b");
        dictionary.put("CTA", "c");
        dictionary.put("CTC", "d");
        dictionary.put("CTG", "e");
        dictionary.put("CTT", "f");
        dictionary.put("GAA", "g");
        dictionary.put("GAC", "h");
        dictionary.put("GAG", "i");
        dictionary.put("GAT", "j");
        dictionary.put("GCA", "k");
        dictionary.put("GCC", "l");
        dictionary.put("GCG", "m");
        dictionary.put("GCT", "n");
        dictionary.put("GGA", "o");
        dictionary.put("GGC", "p");
        dictionary.put("GGG", "q");
        dictionary.put("GGT", "r");
        dictionary.put("GTA", "s");
        dictionary.put("GTC", "t");
        dictionary.put("GTG", "u");
        dictionary.put("GTT", "v");
        dictionary.put("TAA", "w");
        dictionary.put("TAC", "x");
        dictionary.put("TAG", "y");
        dictionary.put("TAT", "z");
        dictionary.put("TCA", " ");
        dictionary.put("TCC", "STOP");
        dictionary.put("TCG", "STOP");
        dictionary.put("TCT", "STOP");
        dictionary.put("TGA", "STOP");
        dictionary.put("TGC", "STOP");
        dictionary.put("TGG", "STOP");
        dictionary.put("TGT", "STOP");
        dictionary.put("TTA", "STOP");
        dictionary.put("TTC", "STOP");
        dictionary.put("TTG", "STOP");
        dictionary.put("TTT", "STOP");
        String output = "";

        //n'accepte que les strings multiples de 3
        if (input.length() % 3 == 0) {
            for (int i = 0; i<input.length()/3;i=i+1){
               //output = output + dictionary.get(letter.toString().toUpperCase());
                input = input.toUpperCase();
                output = output + dictionary.get(input.substring(i*3,(i+1)*3));
            }
        }
        else
        {
            output = "error";
        }
        return output;
    }


        public void onClick(View v) {
            EditText vue = (EditText) v;
            if (vue.getText().toString().length() >= 1) {
                switch (vue.getId()) {
                    case R.id.input_name:
                        mOutputDNA.setText(textToDNA(vue.getText().toString()));
                        break;
                    case R.id.input_dna:
                        mOutputNameView.setText(DNAtoText(vue.getText().toString()));
                        if (mOutputNameView.getText().equals(mInputNameView.getText().toString())) {
                            mImageResult.setImageDrawable(getDrawable(R.drawable.happy_48dp));
                        }
                        else
                        {
                            mImageResult.setImageDrawable(getDrawable(R.drawable.unhappy_48dp));
                        }
                        break;
                    default:
                        break;
                }
            }
        };



}