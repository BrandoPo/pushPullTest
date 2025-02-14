class Model {

        constructor(){

            app.get('/', (req, res) => {
            
                    const config = {
                        server: 'localhost',
                        database: 'drinks_script'
                    };
            
                    connect(config, function (err) {
            
            
                        let request = new Request();
            
            
                        const newLocal = 'select * from drinks';
                        request.query(newLocal,
                            function (err, records) {
            
                                if (err) console.log(err);
            
            
                                res.send(records);
            
                            });
                    });
                });
        }

}

class View {
    constructor (){
        this.form.addEventListener('submit', event => {
            event.preventDefault()
      
            if (xhr.readyState === 200) {
                xhr.POST();
            }
          })
    }
}

class Controller {
    constructor (model, view){
        this.model=model;
        this.view=view;
        const xhr = new XMLHttpRequest();


        xhr.onload = function GET ()  {
            
            xhr.open("GET", "https://github.com/BrandoPo/MVC-Bartender-Application/blob/main/drinks_script.sql");
            xhr.send();
            return xhr;
        };
        xhr.onload = function POST ()  {
            
            xhr.open("POST", "https://github.com/BrandoPo/MVC-Bartender-Application/blob/main/drinks_script.sql");
            xhr.send();
            return xhr;
        };
    }
}

const app = new Controller(new Model(), new View())
