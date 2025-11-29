import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from '../../models/user';
import { NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-roles-modal',
    templateUrl: './roles-modal.component.html',
    styles: ``,
    imports: [NgFor, FormsModule]
})
export class RolesModalComponent implements OnInit {
  @Output() updateSelectedRoles = new EventEmitter();
  user!: User;
  roles!: any[];
  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit() {}

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }
}
