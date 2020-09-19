import { Component, OnInit, Input } from '@angular/core';
import { ComponentFormValues, IComponent } from 'src/app/shared/models/products';
import { AdminService } from '../admin.service';
import { ToastrService } from 'ngx-toastr';
import {HttpEvent, HttpEventType} from '@angular/common/http';

@Component({
  selector: 'app-edit-component-photo',
  templateUrl: './edit-component-photo.component.html',
  styleUrls: ['./edit-component-photo.component.scss']
})
export class EditComponentPhotoComponent implements OnInit {
  @Input() component: IComponent;
  progress = 0;
  addPhotoMode = false;

  constructor(private adminService: AdminService, private toast: ToastrService) { }

  ngOnInit(): void {

  }

  addPhotoModeToggle() {
    this.addPhotoMode = !this.addPhotoMode;
  }

  uploadFile(file: File) {
    this.adminService.uploadComponentImage(file, this.component.id).subscribe((event: HttpEvent<any>) => {
      switch (event.type) {
        case HttpEventType.UploadProgress:
          this.progress = Math.round(event.loaded / event.total * 100);
          break;
        case HttpEventType.Response:
          this.component = event.body;
          setTimeout(() => {
            this.progress = 0;
            this.addPhotoMode = false;
          }, 1500);
      }
    }, error => {
      if (error.errors) {
        this.toast.error(error.errors[0]);
      } else {
        this.toast.error('Problem uploading image');
      }
      this.progress = 0;
    });
  }



  deletePhoto(componentId:number, photoId: number) {
    this.adminService.deleteComponentPhoto(componentId, photoId ).subscribe(() => {
      //const photoIndex = this.product.photos.findIndex(x => x.id === photoId);
    //  this.component.photo = null;
    }, error => {
      this.toast.error('Problem deleting photo');
      console.log(error);
    });
  }

}
