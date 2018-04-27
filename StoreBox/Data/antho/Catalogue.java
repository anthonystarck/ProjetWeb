
public class Catalogue {

    private static Catalogue catalogueInstance = null;
    private ArrayList<Consultable> Items;

    private Catalogue(){
        super();
    }

    public static Catalogue getInstance(){
        if (catalogueInstance == null){
            catalogueInstance = new Catalogue();
            
        }
        return catalogueInstance;        
    }

    public void ajoutCatalogue(Consultable c){

        this.Items.add(c);
    }

    public boolean verificationArticle(Article a){

        if(isValid(a)){

            return true;
        }
        else return false;

    }
    
    public Consultable consulteItem (int id){

        return this.Items.getIndex(id);
    }

    public Boolean verifDispo(Article a) {

        if(a.getQuantite > 0){
            return true;
        }
        else return false;

    }
    public ArrayList<Consultable> rechercheConsultable(String categorie, GPSPosition position, String adresse){

        ArrayList<Consultable> resultat = this.Items.search(categorie, position, adresse);
        return resultat;
    }
 
}