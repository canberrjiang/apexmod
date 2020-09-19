import { Component, Input, OnInit } from '@angular/core';

// import { ColumnMode } from 'projects/swimlane/ngx-datatable/src/public-api';

@Component({
  selector: 'app-datatable',
  templateUrl: './datatable.component.html',
  styleUrls: ['./datatable.component.scss'],
})
export class DatatableComponent implements OnInit {
  rows = [];
  selected = [];
  loadingIndicator = true;
  reorderable = true;
  resizeable = false;

  @Input() categories:any;
  pickCategoryId: number;

  columns = [
    { prop: 'id' },
    { name: 'name' },
    { name: 'productCategory' },
    { name: 'price' },
    { name: 'isDefault', sortable: false },
  ];

  // ColumnMode = ColumnMode;

  constructor() {
    
    this.fetch((data) => {
      this.rows = data;
      setTimeout(() => {
        this.loadingIndicator = false;
      }, 1500);
    });
  }

  fetch(cb) {
    const req = new XMLHttpRequest();
    req.open('GET', `assets/data/company.json`);

    req.onload = () => {
      cb(JSON.parse(req.response));
    };

    req.send();
  }

  // constructor() { }

  // onSelect(selected) {
  //   console.log(selected);
  // }

  deleteProduct(id) {
    console.log(id);

    const resultIndex = this.rows.findIndex((item, index, items) => {
      return item.id === id;
    });

    if (resultIndex === -1) {
      return this.rows;
    }

    let rows = [...this.rows];
    rows.splice(resultIndex, 1);
    this.rows = rows;

    console.log(this.rows);
  }

  setDefault(id){
    const resultIndex = this.rows.findIndex( (item,index, items) => { 
        return item.id === id} );
    this.rows[resultIndex].isDefault = !this.rows[resultIndex].isDefault
    let rows = [...this.rows];
    this.rows = rows;
  }

  RenderChildProduct(){
    console.log(this.pickCategoryId);
    // this.getAllChildProduct().subscribe((response)=>{
    //   console.log(response)
    // })
    
  }

  // getAllChildProduct(){
  //   return this.http.get(this.baseUrl+ '/products/discriminator/childproduct');
  // }


  onActivate(event) {
    console.log('Activate Event', event);
  }

  handleAddChildProduct(){
    
  }

  ngOnInit(): void {}
}
