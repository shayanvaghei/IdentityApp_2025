import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { skip, debounceTime } from 'rxjs';
import { AdminService } from '../../../../admin/admin.service';

@Component({
  selector: 'app-ia-pagination-search-input',
  imports: [ReactiveFormsModule],
  templateUrl: './ia-pagination-search-input.html',
  styleUrl: './ia-pagination-search-input.scss'
})
export class IaPaginationSearchInput implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  @Output() searchOutput = new EventEmitter<string>();

  constructor(private adminService: AdminService,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.adminService.resetFilter$.subscribe(_ => {
      this.searchForm.controls['search'].setValue('');
    });
  }

  initializeForm() {
    this.searchForm = this.formBuilder.group({
      search: ['']
    });

    this.searchForm.get('search')?.valueChanges
      .pipe(
        skip(1), // Skip the initial emission
        debounceTime(500))
      .subscribe(value => {
        this.searchOutput.emit(value);
      });
  }
}
