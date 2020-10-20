import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import {ImageCroppedEvent, base64ToFile} from 'ngx-image-cropper';


@Component({
  selector: 'app-photo-widget',
  templateUrl: './photo-widget.component.html',
  styleUrls: ['./photo-widget.component.scss']
})
export class PhotoWidgetComponent implements OnInit {
  @Output() addFile = new EventEmitter();
  files: File[] = [];
  imageChangedEvent: any = '';
  croppedImage: any = '';

  constructor() { }

  ngOnInit(): void {
  }


  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
    // console.log(1, event);
  }

  imageCropped(event: ImageCroppedEvent) {
    // console.log(1, this.imageChangedEvent);
    // console.log(this.files);
    this.croppedImage = event.base64;
    // console.log(2, event);
    // console.log(3, this.croppedImage)
  }

  onSelect(event) {
    this.files = [];
    this.files.push(...event.addedFiles);
    this.fileChangeEvent(this.files[0]);
  }

  onUpload() {
    // console.log(base64ToFile(this.croppedImage));
    this.addFile.emit(base64ToFile(this.croppedImage));  }
}